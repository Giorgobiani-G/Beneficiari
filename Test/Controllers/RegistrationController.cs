using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;
using Test.Models;
using Test.Security;

namespace Test.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly BenDbContext  _benDbContext;
        public RegistrationController(BenDbContext bdContext)
        {
            _benDbContext = bdContext;
        }
        public IActionResult Registracia()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registracia(Registration reg )
        {
            string encpasswprd = EncDec.Encrypt(reg.Password);

            var exists = (from Rg in _benDbContext.Registrations
                where Rg.Username == reg.Username &&
                      Rg.Password == encpasswprd
                      select Rg).Any();

            if (exists)
            {
                ViewBag.Message = "ასეთი ლოგინით და პაროლით რეგისტრირებული მომხმარებელი უკვე არსებობს";
                RedirectToAction("Registracia", "Registration");
                ModelState.Clear();
            }
            else
            {
                Registration reencpass = new Registration
                {
                    Username = reg.Username,
                    Password = encpasswprd,
                };

                _benDbContext.Registrations.Add(reencpass);
                _benDbContext.SaveChanges();
                ViewBag.Message = "თქვენ წარმატებით გაიარეთ რეგისტრაცია";

               RedirectToAction("Registracia", "Registration");
               ModelState.Clear();
            }

            return View();
        }
    }
}
