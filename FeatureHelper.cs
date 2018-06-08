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
        public static IEnumerable<Feature> GetFeature(int? userId )
        {
            
            List<Feature> features= null;
            using (FeatureService srv = new FeatureService())
            {
                features = srv.GetFeats(userId);
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