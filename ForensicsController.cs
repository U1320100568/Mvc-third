using IntelligenceCloud.Helpers;
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
    public class ForensicsController : Controller
    {
        private ForensicService foreSrv;
        private AttachmentService attaSrv;
        

        public ForensicsController() {
            foreSrv = new ForensicService();
            attaSrv = new AttachmentService();
        }

        // GET: Forensics
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexContact(int id,int? page, string orderer, bool? orderDescend)
        {
            //id =  attachid
            page = page == null ? 1 : page;
            

            var query = foreSrv.contService.Search(s => s.AttachmentId == id);
            var result = foreSrv.Sort<ForensicContact>(query, orderer, orderDescend);//排序
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

        public ActionResult IndexSms(int id, int? page, string orderer ,bool? orderDescend)
        {
            //id =  attachid
            page = page == null ? 1 : page;
           
            var query = foreSrv.smsService.Search(s => s.AttachmentId == id);

            var result = foreSrv.Sort<ForensicSMS>(query, orderer, orderDescend);//排序
            if(orderDescend != true)
            {
                ViewBag.OrderAscend = true;
                
            }
            ViewBag.Orderer = orderer;
            if (result != null)
            {
                
                return View(result.ToPagedList((int)page,20));
            }
            else
            {
                return Content("<script type='text/javascript'>history.back()</script>");
            }
        }

        public ActionResult Export(int id)
        {
            //id =  attachid
            Attachment attach = attaSrv.Get(a => a.AttachmentId == id);
            byte [] result = foreSrv.Export(attach);
            if(result == null)
            {
                return RedirectToAction("IndexSms", id);
            }
            attach.DownloadTime = DateTime.Now;
            attaSrv.Update(attach);
            return File(result, System.Net.Mime.MediaTypeNames.Application.Octet, attach.AttachmentOriginName);
        }

        public ActionResult EditSms(int id)
        {
            //id= ForensicSmsId
            var sms = foreSrv.smsService.Get(s => s.ForensicSMSID == id);
            return View(sms);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSms(ForensicSMS sms)
        {
            //id= ForensicSmsId
            foreSrv.smsService.Update(sms);
            return RedirectToAction("IndexSms",new { id = sms.AttachmentId });
        }

        public ActionResult EditConta(int id)
        {
            //id= ForensicContaId
            var con = foreSrv.contService.Get(s => s.ForensicContactID == id);
            return View(con);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConta(ForensicContact con)
        {
            //id= ForensicContaId
            foreSrv.contService.Update(con);
            return RedirectToAction("IndexContact", new { id = con.AttachmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSms(int ForensicSMSID )
        {
            //id = ForensicSmsId
            var sms = foreSrv.smsService.Get(s => s.ForensicSMSID == ForensicSMSID);
            foreSrv.smsService.Delete(sms);
            return RedirectToAction("IndexSms", new { id =  sms.AttachmentId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConta(int ForensicContactID)
        {
            //id = ForensicContaId
            var con = foreSrv.contService.Get(s => s.ForensicContactID == ForensicContactID);
            foreSrv.contService.Delete(con);
            return RedirectToAction("IndexContact", new { id = con.AttachmentId});
        }

        
    }
}
