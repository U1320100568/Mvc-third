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
        private MemberService accountService;

        public HomeController()
        {
            accountService = new MemberService();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            //test
            Member member = new Member()
            {
                MemberAccount = "admin",
                MemberPwd = "123"
            };
            accountService.Login(member);
            //test

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