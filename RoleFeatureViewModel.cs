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

    public class RoleMemberViewModel
    {
        public List<Member> Members { get; set; }
        public List<Role> Roles { get; set; }
        public List<RoleViewModel> RoleMembers { get; set; }
        public int? CurrentMemberId { get; set; }

    }

    public class RoleViewModel
    {
        public RoleMember RoleMem { get; set; }
        public string RoleName { get; set; }
    }
}