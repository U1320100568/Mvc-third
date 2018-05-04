using IntelligenceCloud.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Services
{
    public class RoleFeatureService : CrudGenericService<RoleFeature>, IDisposable
    {
        private RoleService roleService;
        
        public RoleFeatureService()
        {
            roleService = new RoleService();
           
        }
        
        public bool ControllerAccessible(int memberId,string name)
        {
            return GetFeature(memberId).Any(f => f.ControllerName == name);
            
        }

        public bool ActionAccessible(int memberId, string name)
        {
            return GetFeature(memberId).Any(f => f.ActionName == name);
            
        }
        /*
        public RoleFeature GetFirstOrDefault(int? memberId)
        {
            if(memberId != null)
            {
                RoleFeature feature = GetFeature((int)memberId)
                .Select(r => new RoleFeature { FeatureId =r.FeatureId 
                , ControllerName = r.ControllerName
                , ActionName =r.ActionName
                , RoleNum = (int)r.RoleNum})
                .FirstOrDefault();
                return feature;
            }
            else
            {
                return null;
            }
        }
        */
        /**/
        public IQueryable<RoleAndFeatureViewModel> GetFeature(int? memberId)
        {
            /*
            List<GetRoleAndFeature_Result> rolesFromSp;

            using (IntelligenceCloudEntities ctx = new IntelligenceCloudEntities())
            {
                rolesFromSp = ctx.GetRoleAndFeature(memberId).ToList();
                
            }

            var role = rolesFromSp.Select(r => new RoleAndFeatureViewModel()
            {
                ActionName = r.ActionName,
                ControllerName = r.ControllerName,
                FeatureId = (int)r.FeatureId,
                RoleLock = r.RoleLock,
                Description = r.Description,
                MemberId = (int)r.MemberId,
                RoleNum = r.RoleNum

            }).Where(r => r.RoleLock == false).AsQueryable();

            return role;
            */
            IQueryable<RoleAndFeatureViewModel> a = null;
            return a;
        }
        
        public void CreateRole (CreateRoleViewModel viewModel)
        {
            int RoleNum;
            using (IntelligenceCloudEntities ctx = new IntelligenceCloudEntities())
            {
                RoleNum = ctx.GetRoleNum();
            }

                //int? RoleNum = roleService.GetAll().Max(r => r.RoleNum) + 1;
            Role role = new Role()
            {
                /*
                MemberId = viewModel.RoleAndFeatureViewModel.FirstOrDefault().MemberId,
                RoleLock = false,
                Description = viewModel.RoleAndFeatureViewModel.FirstOrDefault().Description,
                RoleNum = RoleNum,
                isDeleted = false,
                */
            };
            roleService.Create(role);

            foreach( var roleAndFeature in viewModel.RoleAndFeatureViewModel)
            {
                /*
                RoleFeature feature = new RoleFeature() {
                    isDeleted=false,
                    RoleNum = RoleNum,
                    ActionName = roleAndFeature.ActionName,
                    ControllerName = roleAndFeature.ControllerName
                
                };
                
                Create(feature);*/
            }
            
            
            

            
        }
        /*
        public IQueryable<RoleAndFeatureViewModel> GetRoleAndMember(int? roleNum)
        {
            
            List<GetRoleAndMember_Result> rolesFromSp;

            using (IntelligenceCloudEntities ctx = new IntelligenceCloudEntities())
            {
                rolesFromSp = ctx.GetRoleAndMember(roleNum).ToList();
            }

            var role = rolesFromSp.Select(r => new RoleAndFeatureViewModel()
            {
                ActionName = r.ActionName,
                ControllerName = r.ControllerName,
                FeatureId = (int)r.FeatureId,
                RoleLock = r.RoleLock,
                Description = r.Description,
                MemberId = (int)r.MemberId,
                RoleNum = r.RoleNum

            }).Where(r => r.RoleLock == false).AsQueryable();

            return role;
        }
        
        */



    }
    
}