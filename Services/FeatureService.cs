using IntelligenceCloud.Models;
using IntelligenceCloud.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Services
{
    public class FeatureService : CrudGenericService<Feature>
    {
        private RoleService roleSrv;
        private CrudGenericService<RoleFeature> rfSrv;
        public FeatureService()
        {
            roleSrv = new RoleService();
            rfSrv = new CrudGenericService<RoleFeature>();
        }

        public IQueryable<FeatureViewModel> GetRoleFeat(int id)
        {
            List<RoleFeature> roleFeat;
            
            using (IntelligenceCloudEntities ctx = new IntelligenceCloudEntities())
            {
                roleFeat = ctx.RoleFeature.Where(rf => rf.RoleId == id).ToList();
            }
            var result = roleFeat.Select(rf =>
                    new FeatureViewModel()
                    {
                        RoleFeat = rf,
                        FeatName = Get(f => f.FeatureId == rf.FeatureId).FName
                        

                    });
            return result.AsQueryable();
        }

        public IQueryable<Feature> GetRestFeat(int id)
        {
            List<int> restFeat;
            using (IntelligenceCloudEntities ctx = new IntelligenceCloudEntities())
            {
                restFeat = ctx.RoleFeature.Where(rf => rf.RoleId == id).Select(rf => rf.FeatureId).ToList();
                
            }
            return GetAll().Where(f => !restFeat.Contains(f.FeatureId));
        }
        
        public void RoleRemoveFeature( int roleFeatureId)
        {
            var rf = rfSrv.Get(i => i.RFNum == roleFeatureId);
            rfSrv.Delete(rf);
        }

        public void RoleAddFeature(int roleId, int FeatureId)
        {
            RoleFeature rf = new RoleFeature() {
                FeatureId = FeatureId,
                RoleId = roleId
            };
            rfSrv.Create(rf);
        }
    }
}