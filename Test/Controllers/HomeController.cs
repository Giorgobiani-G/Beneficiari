using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly BenDbContext _benDb;

        public HomeController(ILogger<HomeController> logger, BenDbContext dbContext, IWebHostEnvironment environment)
        {
            _logger = logger;
            _benDb = dbContext;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Beneficiarebi(string searchText, int pageSize, int pg = 1)
        {
            var list = from bn in _benDb.Beneficiaris
                       orderby bn.Benid descending
                       select bn;

            IEnumerable<Beneficiari> data = await list.ToListAsync();

            if (!string.IsNullOrEmpty(searchText))
            {

                bool isInt = int.TryParse(searchText, out int resInt);

                if (isInt)
                {
                    data = await list.Where(x => x.Asaki == resInt || x.Telefoni == resInt || x.Piradobisnomeri.Contains(searchText)).ToListAsync();
                }
                else
                {
                    data = await list.Where(n => n.Saxeli.Contains(searchText) || n.Gvari.Contains(searchText)
                   || n.Piradobisnomeri.Contains(searchText) || n.Misamarti.Contains(searchText)
                   || (n.Saxeli + " " + n.Gvari).Contains(searchText) || (n.Gvari + " " + n.Saxeli).Contains(searchText)).ToListAsync();

                    bool isDateTime = DateTime.TryParse(searchText, out DateTime resDateTime);

                    if (isDateTime)
                        data = await list.Where(x => x.Tarigi == resDateTime || x.DabTarigi == resDateTime).ToListAsync();
                }
            }

            #region Paging
            if (pg < 1)
                pg = 1;

            if (pageSize == 0)
                pageSize = 10;

            ViewBag.Page = pageSize;

            int benefCount = data.Count();
            var pager = new Pager(benefCount, pg, pageSize);
            int benfSkip = (pg - 1) * pageSize;

            data = data.Skip(benfSkip).Take(pager.PageSize);

            ViewBag.Pager = pager;

            ViewBag.CurrentFilter = searchText;

            TempData["page"] = pg;

            ViewBag.PageSizes = GetPageSizes(pageSize);
            #endregion

            return View(data);
        }

        private List<SelectListItem> GetPageSizes(int selectedPageSize)
        {
            var pageSizes = new List<SelectListItem>();

            for (int i = 10; i <= 50; i += 10)
            {
                if (i == selectedPageSize)
                {
                    pageSizes.Add(new SelectListItem(i.ToString(), i.ToString(), true));
                }
                else
                {
                    pageSizes.Add(new SelectListItem(i.ToString(), i.ToString()));
                }
            }

            return pageSizes;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ben = await _benDb.Beneficiaris.FindAsync(id);

            if (ben == null)
            {
                return NotFound();
            }
            return View(ben);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Beneficiari beneficiari, int pg)
        {
            _benDb.Update(beneficiari);
            await _benDb.SaveChangesAsync();
            return RedirectToAction(nameof(Beneficiarebi), new { pg });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var delben = await (from De in _benDb.Beneficiaris
                                where De.Benid == id
                                select De).FirstOrDefaultAsync();
            return View(delben);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, int pg)
        {
            var delben = await (from De in _benDb.Beneficiaris
                                where De.Piradobisnomeri == id
                                select De).FirstOrDefaultAsync();
            _benDb.Beneficiaris.Remove(delben);
            _benDb.SaveChanges();

            return RedirectToAction(nameof(Beneficiarebi), new { pg });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult NewVisit()
        {
            var nvisit = (from vs in _benDb.Beneficiaris
                          select new { Name = vs.Saxeli + " " + vs.Gvari + " " + "პ/ნ" + vs.Piradobisnomeri, Pirad = vs.Piradobisnomeri }).ToList();

            ViewBag.Data = nvisit.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Pirad
            }
            ).ToList();

            var v = _benDb.Visits.Any();

            if (!v)
            {
                ViewBag.Max = 1;
            }
            else
            {
                ViewBag.Max = _benDb.Visits.Max(o => o.Vsid) + 1;
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> NewVisit(string personalId, string tipi, DateTime tarigdro, string simptomebi, string mdgomareoba)
        {

            var benresult = (from ben in _benDb.Beneficiaris
                             where ben.Piradobisnomeri == personalId
                             select ben).FirstOrDefault();

            Visit vs = new Visit
            {
                Gvari = benresult.Gvari,
                Saxeli = benresult.Saxeli,
                Piradoba = benresult.Piradobisnomeri,
                VistisTipi = tipi,
                TarigiDro = tarigdro,
                Symptomi = simptomebi,
                Currentuser = LoginController.user,
                Mdgomareoba = mdgomareoba
            };

            _benDb.Visits.Add(vs);

            await _benDb.SaveChangesAsync();

            return RedirectToAction(nameof(NewVisit));
        }


        public async Task<IActionResult> ListVisit(string SearchText, int pg = 1)
        {
            var v = from vs in _benDb.Visits
                    select vs;

            const int pagesize = 5;

            if (pg < 1)
            {
                pg = 1;
            }

            int benefcount = v.Count();
            var pager = new Pager(benefcount, pg, pagesize);
            int benfskip = (pg - 1) * pagesize;

            var data = await v.Skip(benfskip).Take(pager.PageSize).ToListAsync();

            ViewBag.Pager = pager;

            TempData["page"] = pg;

            if (!string.IsNullOrEmpty(SearchText))
            {
                v = (IOrderedQueryable<Visit>)_benDb.Visits.Where(n => n.Saxeli.Contains(SearchText) || n.Gvari.Contains(SearchText) || n.Piradoba.Contains(SearchText) || n.Symptomi.Contains(SearchText) || n.VistisTipi.Contains(SearchText) || n.Mdgomareoba.Contains(SearchText) || (n.Saxeli + " " + n.Gvari).Contains(SearchText) || (n.Gvari + " " + n.Saxeli).Contains(SearchText));
                return View(await v.ToListAsync());
            }
            else
            {
                return View(data);
            }
        }


        public IActionResult Registracia()
        {
            return View("~/Views/Test/Registration.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Registracia(Beneficiari beneficiari)
        {
            Beneficiari b = new Beneficiari
            {
                Piradobisnomeri = beneficiari.Piradobisnomeri,
                Saxeli = beneficiari.Saxeli,
                Gvari = beneficiari.Gvari,
                Misamarti = beneficiari.Misamarti,
                Telefoni = beneficiari.Telefoni,
                Tarigi = beneficiari.Tarigi,
                DabTarigi = beneficiari.DabTarigi
            };

            _benDb.Add(b);

            await _benDb.SaveChangesAsync();

            ModelState.Clear();

            return View("~/Views/Test/Registration.cshtml");
        }


        public async Task<IActionResult> VisitEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vis = await _benDb.Visits.FindAsync(id);

            if (vis == null)
            {
                return NotFound();
            }
            return View(vis);
        }

        [HttpPost]
        public async Task<IActionResult> VisitEdit(Visit vis, int pg)
        {
            _benDb.Update(vis);

            await _benDb.SaveChangesAsync();

            return RedirectToAction(nameof(ListVisit), new { pg });
        }

        public async Task<IActionResult> VisDelete(int id)
        {
            var delvis = await (from De in _benDb.Visits
                                where De.Vsid == id
                                select De).FirstOrDefaultAsync();
            return View(delvis);
        }


        [HttpPost]
        public async Task<IActionResult> VisDelete(string id, int pg)
        {
            var delvis = await (from De in _benDb.Visits
                                where De.Piradoba == id
                                select De).FirstOrDefaultAsync();
            _benDb.Visits.Remove(delvis);
            _benDb.SaveChanges();

            return RedirectToAction(nameof(ListVisit), new { pg });
        }

        public IActionResult BenefToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Beneficiarebi");
                var currentrow = 1;
                worksheet.Cell(currentrow, 1).Value = "პ/ნ";
                worksheet.Cell(currentrow, 2).Value = "სახელი";
                worksheet.Cell(currentrow, 3).Value = "გვარი";
                worksheet.Cell(currentrow, 4).Value = "ასაკი";
                worksheet.Cell(currentrow, 5).Value = "მისამართი";
                worksheet.Cell(currentrow, 6).Value = "დაბ.თარიღი";
                worksheet.Cell(currentrow, 7).Value = "თარიღი";
                worksheet.Cell(currentrow, 8).Value = "ტელეფონი";

                foreach (var beneficiari in _benDb.Beneficiaris)
                {
                    currentrow++;

                    worksheet.Cell(currentrow, 1).Value = "'" + beneficiari.Piradobisnomeri;
                    worksheet.Cell(currentrow, 2).Value = beneficiari.Saxeli;
                    worksheet.Cell(currentrow, 3).Value = beneficiari.Gvari;
                    worksheet.Cell(currentrow, 4).Value = beneficiari.Asaki;
                    worksheet.Cell(currentrow, 5).Value = beneficiari.Misamarti;
                    worksheet.Cell(currentrow, 6).Value = beneficiari.DabTarigi;
                    worksheet.Cell(currentrow, 7).Value = beneficiari.Tarigi;
                    worksheet.Cell(currentrow, 8).Value = beneficiari.Telefoni;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BeneficiariInfo.xlsx");
                }
            }
        }

        public IActionResult VisitToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Visitorebi");
                var currentrow = 1;
                worksheet.Cell(currentrow, 1).Value = "ვიზ#";
                worksheet.Cell(currentrow, 2).Value = "პ/ნ";
                worksheet.Cell(currentrow, 3).Value = "სახელი";
                worksheet.Cell(currentrow, 4).Value = "გვარი";
                worksheet.Cell(currentrow, 5).Value = "ვიზიტის ტიპი";
                worksheet.Cell(currentrow, 6).Value = "ვიზიტის თარიღი";
                worksheet.Cell(currentrow, 7).Value = "სიმპტომი";
                worksheet.Cell(currentrow, 8).Value = "იუზერი";
                worksheet.Cell(currentrow, 9).Value = "მდგომარეობა";

                foreach (var visitorebi in _benDb.Visits)
                {
                    currentrow++;
                    worksheet.Cell(currentrow, 1).Value = visitorebi.Vsid;
                    worksheet.Cell(currentrow, 2).Value = "'" + visitorebi.Piradoba;
                    worksheet.Cell(currentrow, 3).Value = visitorebi.Saxeli;
                    worksheet.Cell(currentrow, 4).Value = visitorebi.Gvari;
                    worksheet.Cell(currentrow, 5).Value = visitorebi.VistisTipi;
                    worksheet.Cell(currentrow, 6).Value = visitorebi.TarigiDro;
                    worksheet.Cell(currentrow, 7).Value = visitorebi.Symptomi;
                    worksheet.Cell(currentrow, 8).Value = visitorebi.Currentuser;
                    worksheet.Cell(currentrow, 9).Value = visitorebi.Mdgomareoba;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "VisitorebiInfo.xlsx");
                }
            }
        }

        public IActionResult PrintPdf(int id)
        {
            var dbvstor = (from vstor in _benDb.Visits.Where(o => o.Vsid == id)
                           select vstor).FirstOrDefault();

            #region
            //var pdfDoc = new Document(new Rectangle(21f,29.7f), 40f, 40f, 60f, 60f);
            //string Doc = $"C:\\Users\\Syllogia\\Desktop\\Visit.pdf";
            //PdfWriter.GetInstance(pdfDoc,new FileStream(Doc, FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None));
            //pdfDoc.Open();

            //var imagepath = @"C:\Users\Syllogia\source\repos\Test\Test\wwwroot\Image\Logo.png";

            //using (FileStream FileStream = new FileStream(imagepath,FileMode.Open))
            //{
            //    var png = Image.GetInstance(System.Drawing.Image.FromStream(FileStream), ImageFormat.Png);
            //    png.ScalePercent(5f);
            //    png.SetAbsolutePosition(pdfDoc.Left,pdfDoc.Top);

            //    pdfDoc.Add(png);

            //}

            //var spacer = new Paragraph("")
            //{
            //    SpacingBefore = 10f,
            //    SpacingAfter = 10f
            //};
            //pdfDoc.Add(spacer);



            //var paragraph = new Paragraph("საქველმოქმედო ფონდ „საქართველოს კარიტასი“-ის ჯანმრთელობის დაცვის პროგრამა");

            //pdfDoc.Add(paragraph);

            //var table = new PdfPTable(new[] {1.5f, 1f, 1.5f, 1f})
            //{
            //    HorizontalAlignment = 3,
            //    WidthPercentage = 75,
            //    DefaultCell = { MinimumHeight = 22f}


            //};

            //table.AddCell("ტესტირების სახეობა");
            //table.AddCell("ჩატარების თარიღი");
            //table.AddCell("სახელი გვარი/პირადი ნომერი");
            //table.AddCell("შედეგი");
            //table.AddCell("COVID-19 ანტიგენის სწრაფი ტესტი");
            //table.AddCell(dbvstor.TarigiDro.ToString());
            //table.AddCell(dbvstor.Saxeli + dbvstor.Gvari + dbvstor.Piradoba);
            //table.AddCell(dbvstor.Mdgomareoba);


            //pdfDoc.Add(table);

            //pdfDoc.Close();
            #endregion
            return View(dbvstor);
        }

        public IActionResult GeneratePdfDocument(int id)

        {
            var dbvstor = (from vstor in _benDb.Visits.Where(o => o.Vsid == id)
                           select vstor).FirstOrDefault();

            //Create a new PDF document.

            PdfDocument doc = new PdfDocument();
            doc.PageSettings.Size = PdfPageSize.A4;

            //Add a page.

            PdfPage page = doc.Pages.Add();

            //Create PDF graphics for the page.

            PdfGraphics graphics = page.Graphics;

            //Create the font for setting the style.
            FileStream fontStream = new FileStream("sylfaen.ttf", FileMode.Open, FileAccess.Read);

            //Use the font installed in the machine

            PdfFont font = new PdfTrueTypeFont(fontStream, 11.45f, PdfFontStyle.Bold);

            //Draw the text.

            graphics.DrawString("საქველმოქმედო ფონდ „საქართველოს კარიტასი“-ის ჯანმრთელობის დაცვის პროგრამა", font, PdfBrushes.Black, new PointF(25f, 250f));

            //Load the image from the disk

            FileStream imageStream = new FileStream("Logo.png", FileMode.Open, FileAccess.Read);

            PdfBitmap image = new PdfBitmap(imageStream);

            //Draw the image

            graphics.DrawImage(image, 200f, 0);

            //Create a PdfLightTable.

            PdfLightTable pdfLightTable = new PdfLightTable();

            //Set the DataSourceType as Direct.

            pdfLightTable.DataSourceType = PdfLightTableDataSourceType.TableDirect;

            //Create columns.

            pdfLightTable.Columns.Add(new PdfColumn("ტესტირების სახეობა"));

            pdfLightTable.Columns.Add(new PdfColumn("ჩატარების თარიღი"));

            pdfLightTable.Columns.Add(new PdfColumn("სახელი გვარი/პირადი ნომერი"));

            pdfLightTable.Columns.Add(new PdfColumn("შედეგი"));

            //Add Rows.

            DateTime tarigi = dbvstor.TarigiDro;

            string saxeligvaripiradoba = dbvstor.Saxeli + " " + dbvstor.Gvari + "/" + dbvstor.Piradoba;
            string shedegi = dbvstor.Mdgomareoba;

            pdfLightTable.Rows.Add(new object[] { "COVID-19 ანტიგენის სწრაფი ტესტი", tarigi, saxeligvaripiradoba, shedegi });

            //create and customize the string formats
            PdfStringFormat format = new PdfStringFormat();
            format.Alignment = PdfTextAlignment.Center;
            format.LineAlignment = PdfVerticalAlignment.Middle;

            //Declare and define the header style.

            PdfCellStyle headerStyle = new PdfCellStyle
            {
                Font = new PdfTrueTypeFont(fontStream, 10f, PdfFontStyle.Bold),
                StringFormat = format
            };

            pdfLightTable.Style.HeaderStyle = headerStyle;

            //Declare and define the alternate style.

            PdfCellStyle altStyle = new PdfCellStyle();
            altStyle.Font = new PdfTrueTypeFont(fontStream, 10f);
            altStyle.StringFormat = format;

            pdfLightTable.Style.DefaultStyle = altStyle;

            //Show header in the table

            pdfLightTable.Style.ShowHeader = true;

            //Draw the PdfLightTable.

            pdfLightTable.Draw(page, 20f, 400f);

            //Draw the text.
            string name = dbvstor.Currentuser;
            graphics.DrawString($"განმახორციელებელი პირი :                    {name}", font, PdfBrushes.Black, new PointF(160f, 650f));

            PdfCreationDateField pdfCreationDateField = new PdfCreationDateField();

            pdfCreationDateField.DateFormatString = DateTime.Now.ToString();

            PdfFont fontfordate = new PdfTrueTypeFont(fontStream, 9f);

            graphics.DrawString($"დაბეჭდვის დრო :  {pdfCreationDateField.DateFormatString}", fontfordate, PdfBrushes.Black, new PointF(350f, 750f));

            //Creating the stream object

            MemoryStream stream = new MemoryStream();

            //Save the PDF document to stream.

            doc.Save(stream);

            //If the position is not set to '0' then the PDF will be empty.

            stream.Position = 0;

            //Close the document.

            doc.Close(true);

            //Defining the ContentType for pdf file.

            string contentType = "application/pdf";

            //Define the file name.

            //string fileName = "Output.pdf";

            //Creates a FileContentResult object by using the file contents, content type, and file name.

            return File(stream, contentType);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
