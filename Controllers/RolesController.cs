using IntelligenceCloud.Models;
using IntelligenceCloud.Services;
using IntelligenceCloud.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntelligenceCloud.Controllers
{
    public class RolesController : Controller
    {

        private RoleService roleSrv;
        private FeatureService featSrv;

        public RolesController() {

            roleSrv = new RoleService();
            featSrv = new FeatureService();
        }

        // GET: Roles
        public ActionResult Index()
        {

            return View(roleSrv.GetAll().ToList());
        }


        // GET: Roles/Create
        public ActionResult CreateEdit(int? id)
        {
            Role role = roleSrv.Get(r => r.RoleId == id);
            if (role != null)
            {
                return PartialView("_PartialCreateEdit", role);
            }
            else
            {
                return PartialView("_PartialCreateEdit", new Role());
            }

        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEdit(Role role)
        {
            if (role.RoleId == 0)
            {
                roleSrv.Create(role);

            }
            else
            {
                roleSrv.Update(role);
            }
            return RedirectToAction("Index");

        }

        public ActionResult EditRoleFeature()
        {
            RoleFeatureViewModel viewModel = new RoleFeatureViewModel() {
                Roles = roleSrv.GetAll().ToList(),
                Features = featSrv.GetAll().ToList()

            };
            return View(viewModel);
        }

        public ActionResult FeatureList(int id)
        {
            RoleFeatureViewModel viewModel = new RoleFeatureViewModel()
            {
                Roles = roleSrv.GetAll().ToList(),
                RoleFeatures = featSrv.GetRoleFeat(id).ToList(),
                Features = featSrv.GetRestFeat(id).ToList(),
                CurrentRoleId = id
               
            };

            return View("EditRoleFeature", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleModifiedFeature( int roleId,int? roleFeatureId, int? featureId)
        {
            if(featureId != null)
            {
                //新增
                featSrv.RoleAddFeature(roleId, (int)featureId);
            }
            if(roleFeatureId != null)
            {
                //刪除
                featSrv.RoleRemoveFeature((int)roleFeatureId);
            }

            return RedirectToAction("FeatureList", new { id = roleId });
        }

        

        public ActionResult GetPartialView()
        {
            return View("_PartialCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Role role = roleSrv.Get(r => r.RoleId == id);
            roleSrv.Delete(role);
            return RedirectToAction("Index");
        }
        


        /*
       
        public ActionResult EditMember(int id)
        {
            //id = RoleNum
            var roleAndMember = roleService.Search(r => r.RoleNum == id).ToList();
            var members = new MemberService();

            //選擇要加入的會員
            var membersName =  members.GetAll()
                .Select(column => new SelectListItem() { Text = column.MemberName, Value = column.MemberId.ToString(), Selected = false });
            var existedName = roleAndMember.Select(r =>  r.MemberId.ToString() );
            membersName = membersName.Where(m => !existedName.Contains(m.Value));
            ViewBag.AssignMember = membersName;

            return View(roleAndMember);
        }
        public ActionResult DeleteMember(int id)
        {
            Role role = roleService.Get(r => r.RoleId == id);
            roleService.Delete(role);
            return RedirectToAction("EditMember",new { id =role.RoleNum });
        }
        public ActionResult LockMember(int id)
        {
            roleService.LockRole(id);
            return RedirectToAction("EditMember", new { id = role.RoleNum });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMember(Role role)
        {
            var validate = roleService.Search(r => r.MemberId == role.MemberId).Where(r => r.RoleNum == role.RoleNum);
            if(!validate.Any())
            {
                roleService.Create(role);
            }
            
            
            return RedirectToAction("EditMember", new { id = role.RoleNum });
        }

        public ActionResult EditFeature(int id)
        {
            //id = RoleNum
            var feature = roleFeatureService.Search(f => f.RoleNum == id).ToList();
            
            return View(feature);
        }

        public ActionResult DeleteFeature(int id)
        {
            var feature = roleFeatureService.Get(f => f.FeatureId == id);
            roleFeatureService.Delete(feature);
            return RedirectToAction("EditFeature", new { id = feature.RoleNum });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFeature(RoleFeature feature)
        {
            roleFeatureService.Create(feature);
            return RedirectToAction("EditFeature", new { id = feature.RoleNum });
        }


        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                roleFeatureService.Dispose();
            }
            base.Dispose(disposing);
        }

        */
    }
}
