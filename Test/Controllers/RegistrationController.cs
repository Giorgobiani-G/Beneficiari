using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly BenDbContext  _benDbContext;
        public RegistrationController(BenDbContext bdContext)
        {
            this._benDbContext = bdContext;
        }
        public IActionResult Registracia()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registracia(Registration reg )
        {
            var exists = (from Rg in _benDbContext.Registrations
                where Rg.Username == reg.Username &&
                      Rg.Password == reg.Password
                select Rg).Any();

            if (exists)
            {
                ViewBag.Message = "ასეთი ლოგინით და პაროლით რეგისტრირებული მომხმარებელი უკვე არსებობს";
                RedirectToAction("Registracia", "Registration");
                ModelState.Clear();
            }
            else
            {


                _benDbContext.Registrations.Add(reg);
                _benDbContext.SaveChanges();
                ViewBag.Message = "თქვენ წარმატებით გაიარეთ რეგისტრაცია";

               RedirectToAction("Registracia", "Registration");
               ModelState.Clear();
            }

            return View();
        }


    }
}
