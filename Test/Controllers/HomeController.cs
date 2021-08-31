using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly BenDbContext _benDb;
        //private Rectangle A4;

        public HomeController(ILogger<HomeController> logger, BenDbContext dbContext,IWebHostEnvironment environment)
        {
            _logger = logger;
            _benDb = dbContext;
            _environment = environment;
        }

        public IActionResult Index()
        {
           
            return View();
        }

        public async Task<IActionResult> Beneficiarebi(string SearchText)
        {

            var vlistBeneficiaris = from bn in _benDb.Beneficiaris
                orderby bn.Benid descending 
                                    select bn;
            

            if (!String.IsNullOrEmpty(SearchText))
            {

                vlistBeneficiaris = (IOrderedQueryable<Beneficiari>)_benDb.Beneficiaris.Where(n => n.Saxeli.Contains(SearchText) || n.Gvari.Contains(SearchText) || n.Piradobisnomeri.Contains(SearchText) || n.Misamarti.Contains(SearchText)||(n.Saxeli + " " + n.Gvari).Contains(SearchText) || (n.Gvari + " " + n.Saxeli).Contains(SearchText));
                return View(await vlistBeneficiaris.ToListAsync());
            }
            else
            {

                return View(await vlistBeneficiaris.ToListAsync());

            }

            return View(vlistBeneficiaris);
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
        public async Task<IActionResult> Edit(Beneficiari beneficiari)

        {





            _benDb.Update(beneficiari);

            await _benDb.SaveChangesAsync();

            //return View("Beneficiarebi");

            return RedirectToAction(nameof(Beneficiarebi));
        }

        public async  Task<IActionResult> Delete(int id)
        {
            var delben = await  (from De in _benDb.Beneficiaris
                where De.Benid == id
                select De).FirstOrDefaultAsync();
            return View(delben);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var delben = await (from De in _benDb.Beneficiaris
                where De.Piradobisnomeri == id
                select De).FirstOrDefaultAsync();
           _benDb.Beneficiaris.Remove(delben);
               _benDb.SaveChanges();

               return RedirectToAction(nameof(Beneficiarebi));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult NewVisit()
        {
            var nvisit = (from vs in _benDb.Beneficiaris
                select new{Name= vs.Saxeli + " " +vs.Gvari + " " +"პ/ნ" + vs.Piradobisnomeri, Pirad=vs.Piradobisnomeri }).ToList();

            ViewBag.Data = nvisit.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Pirad

                }
            ).ToList();

            var v = _benDb.Visits.Any();

            if(!v)
            {
                ViewBag.Max = 1;
            }
            else
            {
                ViewBag.Max = _benDb.Visits.Max(o => o.Vsid)+1;
            }


            return  View();
        }

        
        [HttpPost]
        public async Task<IActionResult> NewVisit(string personalId,string tipi,DateTime tarigdro,string simptomebi, string mdgomareoba)
        {

            var benresult = (from ben in _benDb.Beneficiaris
                     where ben.Piradobisnomeri == personalId
                     select ben).FirstOrDefault();

            


            Visit vs = new Visit();
            vs.Gvari = benresult.Gvari;
            vs.Saxeli = benresult.Saxeli;
            vs.Piradoba = benresult.Piradobisnomeri;
            vs.VistisTipi = tipi;
            vs.TarigiDro = tarigdro;
            vs.Symptomi = simptomebi;
            vs.Currentuser = LoginController.user;
            vs.Mdgomareoba = mdgomareoba;



            _benDb.Visits.Add(vs);

            await _benDb.SaveChangesAsync();
            //await Response.WriteAsync(checkBox.ToString());


            return RedirectToAction(nameof(NewVisit));

            
        }


        public async Task<IActionResult> ListVisit(string SearchText)
        {
            var v = from vs in _benDb.Visits
                select vs;

            if (!String.IsNullOrEmpty(SearchText))
            {
                v = (IOrderedQueryable<Visit>)_benDb.Visits.Where(n => n.Saxeli.Contains(SearchText) || n.Gvari.Contains(SearchText) || n.Piradoba.Contains(SearchText) || n.Symptomi.Contains(SearchText) || n.VistisTipi.Contains(SearchText) || n.Mdgomareoba.Contains(SearchText) || (n.Saxeli + " " + n.Gvari).Contains(SearchText) || (n.Gvari + " " + n.Saxeli).Contains(SearchText));
                return View(await v.ToListAsync());
            }
            else
            {

                return View(await v.ToListAsync());
            }

            return View(v);
        }


        public IActionResult Registracia()
        {
            return View("~/Views/Test/Registration.cshtml");
        }

        [HttpPost]
        public async  Task<IActionResult> Registracia(Beneficiari beneficiari)
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
        public async Task<IActionResult> VisitEdit(Visit vis)

        {





            _benDb.Update(vis);

            await _benDb.SaveChangesAsync();

            //return View("Beneficiarebi");

            return RedirectToAction(nameof(ListVisit));
        }


        public async  Task<IActionResult> VisDelete(int id)
        {
            var delvis = await  (from De in _benDb.Visits
                where De.Vsid == id
                select De).FirstOrDefaultAsync();
            return View(delvis);
        }
        [HttpPost]
        public async Task<IActionResult> VisDelete(string id)
        {
            var delvis = await (from De in _benDb.Visits
                where De.Piradoba == id
                select De).FirstOrDefaultAsync();
           _benDb.Visits.Remove(delvis);
               _benDb.SaveChanges();

               return RedirectToAction(nameof(ListVisit));
        }




        public IActionResult BenefToExcel()
        {
            using(var workbook = new XLWorkbook())
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
                    worksheet.Cell(currentrow, 1).Value = beneficiari.Piradobisnomeri;
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
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","BeneficiariInfo.xlsx");
                }
            }

            return RedirectToAction(nameof(Beneficiarebi));
        }
            
        public IActionResult VisitToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Visitorebi");
                var currentrow = 1;
                worksheet.Cell(currentrow, 1).Value = "ვიზ#";
                worksheet.Cell(currentrow, 2).Value = "სახელი";
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
                    worksheet.Cell(currentrow, 2).Value = visitorebi.Piradoba;
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

            return RedirectToAction(nameof(ListVisit));
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
