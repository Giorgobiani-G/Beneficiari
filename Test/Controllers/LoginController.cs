using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Presentation;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    public class LoginController : Controller
    {
        private readonly BenDbContext _benDbContext;
        public static bool issinged;
        public static string? user;

        public LoginController(BenDbContext benDbContext)
        {
            this._benDbContext = benDbContext;
        }

       
        public IActionResult Login()
        {
           
            return View();
        }
        [HttpPost]
        public IActionResult Login(Login login)
        {
            bool rg = (from reg in _benDbContext.Registrations
                where reg.Username == login.User
                      && reg.Password == login.Password
                select reg).Any();

            if (rg)
            {
                issinged = true;
                user = login.User;
                return RedirectToAction("Beneficiarebi", "Home");
            }
            else
            {
                ViewBag.Message = "არასროი ლოგინი ან პაროლი";
                 RedirectToAction("Login","Login");
                ModelState.Clear();
            }

            
            
            return View();
        }

        public IActionResult Logout()
        {
            issinged = false;
            user = null;
           return RedirectToAction("Index", "Home");

        }
    }
}
