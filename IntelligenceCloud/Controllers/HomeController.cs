using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IntelligenceCloud.Helpers;
using IntelligenceCloud.Infrastructure;
using IntelligenceCloud.Models;
using IntelligenceCloud.Services;

namespace IntelligenceCloud.Controllers
{
   
    public class HomeController : Controller
    {
        private AccountService accountService = new AccountService();
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            
            return View();
        }
        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(Member member)
        {
            Member authorMember= accountService.Login(member);
            if (authorMember == null)
            {
                return View();
            }
            
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            accountService.Logout();
            return RedirectToAction("Login");
        }
    }
}