using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IntelligenceCloud.Infrastructure;
using IntelligenceCloud.Models;
using IntelligenceCloud.Services;

namespace IntelligenceCloud.Controllers
{
    
    public class MembersController : Controller
    {

        private MemberService memberService;

        public MembersController()
        {
            memberService = new MemberService();
        }

        // GET: Members
        [UserAuthorize]
        public ActionResult Index(string searchString)
        {
            return View(memberService.GetAll().ToList());
        }

        [UserAuthorize]
        public ActionResult Search(string searchString)
        {
            //輸入空白的東西
            if (string.IsNullOrEmpty(searchString) || string.IsNullOrWhiteSpace(searchString))
            {
                return View("Index", memberService.GetAll().ToList());
            }
            string searchProp = "MemberName MemberAccount";
            var result = memberService.Search(searchString, searchProp);
            //搜尋無資料
            if (result == null)
            {
                ViewBag.Msg = "查無資料";
                return View("Index", memberService.GetAll().ToList());
            }
            return View("Index", result);
            
        }

        /*原本的
        // GET: Members/Details/5
        [UserAuthorize]
        public ActionResult Details(int? id)
        {
            
            Member member = memberService.Get(m => m.MemberId == id) ;
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }
        */

        // GET: Members/Details/5
        [UserAuthorize]
        public ActionResult Details(int? id)
        {
            Member member = memberService.Get(m => m.MemberId == id);
                
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }


        // GET: Members/Create
        [UserAuthorize]
        public ActionResult Create()
        {   
            return View();
        }

        // POST: Members/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [UserAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MemberId,MemberAccount,MemberName,MemberPwd,RoleId,isDeleted")] Member member)
        {
            if (ModelState.IsValid)
            {
               
                
                memberService.Create(member);
                return RedirectToAction("Index");
            }

            return View(member);
        }

        // GET: Members/Edit/5
        [UserAuthorize]
        public ActionResult Edit(int? id)
        {

            Member member = memberService.Get(m => m.MemberId == id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Members/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserAuthorize]
        public ActionResult Edit([Bind(Include = "MemberId,MemberAccount,MemberName,MemberPwd,RoleId,isDeleted")] Member member)
        {
            if (ModelState.IsValid)
            {
                memberService.Update(member);
                return RedirectToAction("Index");
            }
            return View(member);
        }

        public ActionResult CreateEdit(int? id)
        {
            Member member = memberService.Get(m => m.MemberId == id);
            if(member == null)
            {
                return PartialView("_PartialCreateEdit", new Member());
            }
            else
            {
                return PartialView("_PartialCreateEdit", member);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEdit(Member member)
        {
            if (!ModelState.IsValid)
            {
                 Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Content("ModelState.IsValid");
            }

            if(member.MemberId == 0)
            {
                memberService.Create(member);
            }
            else
            {
                memberService.Update(member);
            }
            return RedirectToAction("Index");
        }

        // GET: Members/Delete/5
        public ActionResult Delete(int? id)
        {

            Member member = memberService.Get(m => m.MemberId == id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Member member = memberService.Get(m => m.MemberId == id);
            member = memberService.DeleteAccountSetting(member);
            memberService.Delete(member);
            //return RedirectToAction("Index");
            return Content("OK");
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                memberService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
