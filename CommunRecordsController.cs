using IntelligenceCloud.Models;
using IntelligenceCloud.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntelligenceCloud.Controllers
{
    public class CommunRecordsController : Controller
    {
        private CommunRecordService communSrv;
         
        public CommunRecordsController()
        {
            communSrv = new CommunRecordService();
        }

        // GET: CommunRecords
        public ActionResult Index(int id,int? page, string orderer, bool? orderDescend)
        {
            //id = AttachmentId
            page = page == null ? 1:page; 
            
            var query = communSrv.commCrud.Search(s => s.AttachmentId == id);
            var result = communSrv.Sort<CommunRecord>(query, orderer, orderDescend);//排序
            if (orderDescend != true)
            {
                ViewBag.OrderAscend = true;

            }
            ViewBag.Orderer = orderer;

            if (result != null)
            {
                return View(result.ToPagedList((int)page, 20));
            }
            else
            {
                return Content("<script type='text/javascript'>history.back()</script>");
            }
            
        }
        
        // GET: CommunRecords/Edit/5
        public ActionResult Edit(int id)
        {
            CommunRecord com = communSrv.commCrud.Get(c => c.CPhoneRecordId == id);
            return View(com);
        }

        // POST: CommunRecords/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CommunRecord com)
        {

            // TODO: Add update logic here
            if (ModelState.IsValid)
            {
                communSrv.commCrud.Update(com);
            }
            return RedirectToAction("Index", new { id = com.AttachmentId});
            
            
        }

        public ActionResult Download(int id)
        {
            Attachment attach = null;
            using (AttachmentService srv = new AttachmentService())
            {
                attach = srv.Get(a => a.AttachmentId == id);
            }
            //id = attachmentId
            byte[] result = communSrv.Export(attach);
            return File(result, "application/vnd.ms-excel", attach.AttachmentOriginName);
        }
        

        // POST: CommunRecords/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var comm = communSrv.commCrud.Get(c => c.CPhoneRecordId == id);
            communSrv.commCrud.Delete(comm);
            return RedirectToAction("Index", new { id = comm.AttachmentId });
            
        }
    }
}
