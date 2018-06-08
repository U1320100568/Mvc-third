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
        private MemberService memSrv;

        public RolesController() {

            roleSrv = new RoleService();
            featSrv = new FeatureService();
            memSrv = new MemberService();
        }

        // GET: Roles
        public ActionResult Index()
        {

            return View(roleSrv.GetAll().ToList());
        }

        //增加和編輯權限，view dialog
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

        //增加和編輯權限
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Role role = roleSrv.Get(r => r.RoleId == id);
            roleSrv.Delete(role);
            return RedirectToAction("Index");
        }

        //列出所有的權限和功能
        public ActionResult EditRoleFeature()
        {
            RoleFeatureViewModel viewModel = new RoleFeatureViewModel() {
                Roles = roleSrv.GetAll().ToList(),
                Features = featSrv.GetAll().ToList()

            };
            return View(viewModel);
        }

        //列出某權限的功能
        public ActionResult FeatureList(int id)
        {
            //id = RoleId
            RoleFeatureViewModel viewModel = new RoleFeatureViewModel()
            {
                Roles = roleSrv.GetAll().ToList(),
                RoleFeatures = featSrv.GetRoleFeat(id).ToList(),
                Features = featSrv.GetRestFeat(id).ToList(),
                CurrentRoleId = id
               
            };

            return View("EditRoleFeature", viewModel);
        }
        

        //增加功能和減少功能
        [HttpPost]
        public ActionResult RoleModifiedFeatures(int roleId, int? roleFeatureId, int? featureId)
        {
            FeatureViewModel roleFeat = new FeatureViewModel();

            if (featureId != null)
            {
                //新增
                featSrv.RoleAddFeature(roleId, (int)featureId);
                roleFeat = featSrv.GetRoleFeat(roleId).FirstOrDefault(rf => rf.RoleFeat.FeatureId == featureId);
            }
            if (roleFeatureId != null)
            {
                //刪除
                roleFeat = featSrv.GetRoleFeat(roleId).FirstOrDefault(rf => rf.RoleFeat.RFNum == roleFeatureId);
                featSrv.RoleRemoveFeature((int)roleFeatureId);
            }
            return Json(new { roleId = roleFeat.RoleFeat.RoleId, roleFeatNum = roleFeat.RoleFeat.RFNum, featId = roleFeat.RoleFeat.FeatureId, featName = roleFeat.FeatName }, JsonRequestBehavior.AllowGet);
            
        }

        //列出所有權限和會員
        public ActionResult EditRoleMember()
        {
            RoleMemberViewModel viewModel = new RoleMemberViewModel()
            {
                Roles = roleSrv.GetAll().ToList(),
                Members = memSrv.GetAll().ToList()
            };
            return View(viewModel);
        }

        //列出某會員的權限
        public ActionResult RoleList(int id)
        {
            //id = MemberId
            RoleMemberViewModel viewModel = new RoleMemberViewModel()
            {
                Roles = roleSrv.GetRestRole(id).ToList(),
                Members = memSrv.GetAll().ToList(),
                RoleMembers = roleSrv.GetRoleMember(id).ToList(),
                CurrentMemberId = id
            };

            return View("EditRoleMember", viewModel);
        }

        //更改權限和會員的配置
        public ActionResult MemberModifiedRoles(int memberId, int? roleMemberNum, int? roleId)
        {
            RoleViewModel roleMem = new RoleViewModel();
            if (roleId != null)
            {
                //新增
                roleSrv.MemberAddRole(memberId, (int)roleId);
                roleMem = roleSrv.GetRoleMember(memberId).FirstOrDefault(v => v.RoleMem.RoleId == roleId);

            }
            if (roleMemberNum != null)
            {
                //刪除
                roleMem = roleSrv.GetRoleMember(memberId).FirstOrDefault(v => v.RoleMem.RMNum == roleMemberNum);
                roleSrv.MemberRemoveRole((int)roleMemberNum);
            }
            
            
            return Json(new { memberId = roleMem.RoleMem.MemberId, roleMemberNum = roleMem.RoleMem.RMNum, roleId = roleMem.RoleMem.RoleId, roleName = roleMem.RoleName }, JsonRequestBehavior.AllowGet);

            
        }
    }
}
