using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IntelligenceCloud.Models;
using IntelligenceCloud.Services;

namespace IntelligenceCloud.Controllers
{
    public class MembersController : Controller
    {
        
        private CrudRepository<Member> memberRepository = new CrudRepository<Member>();

        // GET: Members
        public ActionResult Index(string searchString)
        {
            return View(memberRepository.GetAll().ToList());
        }

        public ActionResult Search(string searchString)
        {
            //輸入空白的東西
            if (string.IsNullOrEmpty(searchString) || string.IsNullOrWhiteSpace(searchString))
            {
                return View("Index", memberRepository.GetAll().ToList());
            }
            string searchProp = "MemberName MemberAccount";
            var result = memberRepository.Search(searchString, searchProp);
            //搜尋無資料
            if (result == null)
            {
                ViewBag.Msg = "查無資料";
                return View("Index", memberRepository.GetAll().ToList());
            }
            return View("Index", result);
            
        }

        // GET: Members/Details/5
        public ActionResult Details(int? id)
        {
            
            Member member = memberRepository.Get(m => m.MemberId == id) ;
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // GET: Members/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Members/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MemberId,MemberAccount,MemberName,MemberPwd,RoleId,isDeleted")] Member member)
        {
            if (ModelState.IsValid)
            {
                AccountService accountService = new AccountService();
                member = accountService.CreateAccountSetting(member);
                memberRepository.Create(member);
                return RedirectToAction("Index");
            }

            return View(member);
        }

        // GET: Members/Edit/5
        public ActionResult Edit(int? id)
        {

            Member member = memberRepository.Get(m => m.MemberId == id);
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
        public ActionResult Edit([Bind(Include = "MemberId,MemberAccount,MemberName,MemberPwd,RoleId,isDeleted")] Member member)
        {
            if (ModelState.IsValid)
            {
                memberRepository.Update(member);
                return RedirectToAction("Index");
            }
            return View(member);
        }

        // GET: Members/Delete/5
        public ActionResult Delete(int? id)
        {

            Member member = memberRepository.Get(m => m.MemberId == id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Member member = memberRepository.Get(m => m.MemberId == id);
            AccountService accountService = new AccountService();
            member = accountService.DeleteAccountSetting(member);
            memberRepository.Delete(member);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                memberRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
