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
            //return View(db.Attachment.ToList());
            return View(attachService.GetAll());

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
            attachmentUse.Add(new SelectListItem { Text = "Cellebrite UFED報告檔", Value = "Cellebrite UFED報告檔" });
            ViewBag.attachmentUse = attachmentUse;

            return View();
        }
        

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

        // GET: Attachments/Edit/5

        public ActionResult Edit(int? id)
        {
            
            Attachment attachment = attachService.Get(a => a.AttachmentId == id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            return View(attachment);
        }

        // POST: Attachments/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AttachmentId,AttachmentPath,AttachmentName,AttachmentType,MemberId,isDeleted,UploadTime,DeletedTime,DownloadTime")] Attachment attachment)
        {
            if (ModelState.IsValid)
            {
                attachService.Update(attachment);
                return RedirectToAction("Index");
            }
            return View(attachment);
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
