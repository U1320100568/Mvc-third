using IntelligenceCloud.Models;
using IntelligenceCloud.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntelligenceCloud.Controllers
{
    public class RolesController : Controller
    {
        private RoleFeatureService roleFeatureService;
        private RoleService roleService;

        public RolesController() {
            roleFeatureService = new RoleFeatureService();
            roleService = new RoleService();
        }

        // GET: Roles
        public ActionResult Index()
        {
            int? input = null;
            var roles = roleFeatureService.GetFeature(input).GroupBy(r => r.RoleNum);
            return View(roles);
        }

        // GET: Roles/Details/5
        public ActionResult Details(int id)
        {
            //id = RoleNum
            var roles = roleFeatureService.GetRoleAndMember(id).GroupBy(r => r.MemberId); 
            return View(roles);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
           
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateRoleViewModel roleAndFeature)
        {

            roleFeatureService.CreateRole(roleAndFeature);
            return RedirectToAction("Index");
            
        }

        public ActionResult GetPartialView()
        {
            return View("_PartialCreate");
        }

        

       
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
            Role role = roleService.Get(r => r.RoleId == id);
            role.RoleLock = (bool)role.RoleLock ? false : true;
            roleService.Update(role);
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
    }
}
