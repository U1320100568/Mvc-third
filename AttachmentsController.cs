using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using IntelligenceCloud.Helpers;
using IntelligenceCloud.Infrastructure;
using IntelligenceCloud.Models;
using IntelligenceCloud.Services;

namespace IntelligenceCloud.Controllers
{
    public class AttachmentsController : Controller
    {
        private AttachmentService attachService;

        public AttachmentsController()
        {
            attachService = new AttachmentService();
        }
        // GET: Attachments
        [UserAuthorize]
        public ActionResult Index()
        {

            return View(attachService.ShowAccessAttach());
            //return View(attachService.GetAll());

        }

        // GET: Attachments/Details/5
        public ActionResult Details(int? id)
        {

            Attachment attachment = attachService.Get(a => a.AttachmentId == id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            var viewModel = attachService.ShowExcelFile(attachment);

            return View(viewModel);
        }



        // GET: Attachments/Create
        [UserAuthorize]
        public ActionResult Create()
        {
            var attachmentUse = new List<SelectListItem>();
            attachmentUse.Add(new SelectListItem { Text = "通聯記錄", Value = "通聯記錄" });
            attachmentUse.Add(new SelectListItem { Text = "Cellebrite UFED報告檔", Value = "Cellebrite UFED報告檔" });//
            ViewBag.attachmentUse = attachmentUse;

            return View();
        }

        ///檔案新增
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserAuthorize]
        public ActionResult Create(AttachViewModel attachViewModel)
        {

            if (attachViewModel.AttachFiles.Count() > 0)
            {
                attachService.CreateViewModelToDatabase(attachViewModel);

            }
            return RedirectToAction("Index");
        }

        public ActionResult CreateaAjax(HttpPostedFileBase[] attachFiles, string attachmentUse, string connId)
        {

            //var httpPostedFile = HttpContext.Request.Files["AttachFiles"];
            string rspStr = string.Empty;
            if(attachFiles.Count() > 0)
            {
                AttachViewModel attachViewModel = new AttachViewModel();
                attachViewModel.AttachFiles = attachFiles;
                attachViewModel.AttachmentUse = attachmentUse;
                attachViewModel.ConnId = connId;
                rspStr = attachService.CreateViewModelToDatabase(attachViewModel);
               
            }

            
            return Content("connectId: "+connId+"  rps: "+ rspStr);
        }

        ///檔案下載
        [UserAuthorize]
        public ActionResult Download(int id)
        {
            Attachment attachment = attachService.Get(f => f.AttachmentId == id);
            byte[] data = attachService.Download(attachment);
            if(data == null)
            {
                 return RedirectToAction("Index");

            }   
            //更新下載時間
            attachment.DownloadTime = DateTime.Now;
            attachService.Update(attachment);
            return File(data, System.Net.Mime.MediaTypeNames.Application.Octet, attachment.AttachmentOriginName);
        }

        //修改檔案共享人
        public ActionResult MemberShared(int? id)
        {
            return PartialView("_MemberShared", attachService.GetMemberShared(id));
        }
        //修改檔案共享人
        public ActionResult MemberSharedAddRemove(int attachId, int memberId)
        {
            Attachment attach = attachService.ModifyMemberShared(attachId, memberId);
            attachService.Update(attach);
            return Json(new { MemberName = IdentityHelper.GetMemberName(memberId), MemberId = memberId });

        }

        

        // GET: Attachments/Delete/5
        public ActionResult Delete(int? id)
        {

            Attachment attachment = attachService.Get(a => a.AttachmentId == id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            return View(attachment);
        }

        // POST: Attachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Attachment attachment = attachService.Get(a => a.AttachmentId == id);
            //更新刪除時間
            attachment.DeletedTime = DateTime.Now;
            attachService.Update(attachment);
            attachService.Delete(attachment);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                attachService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
