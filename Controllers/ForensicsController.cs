using IntelligenceCloud.Services;
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
        private int AttachmentId {get; set;}

        public ForensicsController(int id) {
            foreSrv = new ForensicService();
            AttachmentId = id;
        }

        // GET: Forensics
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexContact()
        {
            return View(foreSrv.contService.Search(c => c.AttachmentId == AttachmentId));
        }

        public ActionResult IndexSms()
        {
            return View(foreSrv.smsService.Search(s => s.AttachmentId == AttachmentId));
        }

        // GET: Forensics/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Forensics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Forensics/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Forensics/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Forensics/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Forensics/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Forensics/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
