using IntelligenceCloud.Models;
using IntelligenceCloud.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntelligenceCloud.Controllers
{
    public class FeaturesController : Controller
    {
        private FeatureService featSrv;

        public FeaturesController()
        {
            featSrv = new FeatureService();
        }

        // GET: Features
        public ActionResult Index()
        {
            return View(featSrv.GetAll().ToList());
        }

        
        

        // GET: Features/Edit/5
        public ActionResult CreateEdit(int? id)
        {
            Feature feat = featSrv.Get(f => f.FeatureId == id);
            if(feat != null)
            {
                return PartialView("_PartialCreateEdit",feat);
            }
            else
            {
                return PartialView("_PartialCreateEdit", new Feature());
            }
        }

        // POST: Features/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEdit(Feature feat)
        {
            if (feat.FeatureId == 0)
            {
                featSrv.Create(feat);
            }
            else
            {
                featSrv.Update(feat);
            }
            return RedirectToAction("Index");
            
        }

        
        // POST: Features/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                Feature feat = featSrv.Get(f => f.FeatureId == id);
                featSrv.Delete(feat);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
