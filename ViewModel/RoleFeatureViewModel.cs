using IntelligenceCloud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.ViewModel
{
    public class RoleFeatureViewModel
    {
        public List<Role> Roles { get; set; }
        public List<Feature> Features { get; set; }
        public List<FeatureViewModel> RoleFeatures { get; set; }
        public int? CurrentRoleId { get; set; }
    }

    public class FeatureViewModel
    {
        public RoleFeature RoleFeat { get; set; }
        public string FeatName { get; set; }
    }
}