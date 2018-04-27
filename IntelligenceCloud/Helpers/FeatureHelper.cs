using IntelligenceCloud.Models;
using IntelligenceCloud.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Helpers
{
    public class FeatureHelper
    {
        public static IQueryable<RoleAndFeatureViewModel> GetFeature(int? userId )
        {
            
            IQueryable<RoleAndFeatureViewModel> features= null;
            if(userId != null)
            {
                using (RoleFeatureService srv = new RoleFeatureService())
                {
                    features = srv.GetFeature(userId);
                }
            }
            
            if(features != null)
            {
                //若此會員有此功能，回傳true
                var query = features.Where(f => f.ActionName == "Index");
                if (query.Any())
                {
                    return query;
                }
            }

            return null;
        }
    }
}