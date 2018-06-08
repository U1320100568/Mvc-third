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


        //判斷會員有無此功能權限
        public bool Accessible(int memberId, string ctrlName,string actName)
        {
            bool result;
            using (IntelligenceCloudEntities   ctx = new IntelligenceCloudEntities())
            {
                var roles = ctx.RoleMember.Where(rm => rm.MemberId == memberId).Select(rm => rm.RoleId);
                var features = ctx.RoleFeature.Where(rf => roles.Contains(rf.RoleId)).Select(rf => rf.FeatureId);
                result = ctx.Feature.Where(f => features.Contains(f.FeatureId)).Any(f => f.ControllerName == ctrlName && f.ActionName == actName);
            }
            return result;
        }

        public List<Feature> GetFeats(int? memberId)
        {
            List<Feature> feats = new List<Feature>();
            if(memberId != null)
            {
                
                using (IntelligenceCloudEntities ctx = new IntelligenceCloudEntities())
                {
                    var roles = ctx.RoleMember.Where(rm => rm.MemberId == memberId).Select(rm => rm.RoleId);
                    var roleFeat = ctx.RoleFeature.Where(rf => roles.Contains(rf.RoleId)).Select(rf => rf.FeatureId);
                    feats = ctx.Feature.Where(f => roleFeat.Contains(f.FeatureId)).ToList();
                }
                return feats;
            }

            return feats;
        }
    }
}