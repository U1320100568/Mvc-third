using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Models
{
    public partial class RoleAndFeatureViewModel
    {
        
        public int FeatureId { get; set; }
        public int? RoleNum { get; set; }
        public int MemberId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool? RoleLock { get; set; }
        public string Description { get; set; }
    }

    public partial class MemberAndFeatureViewModel
    {
        public Member Member { get; set; }
        public IQueryable<IGrouping<int? ,RoleAndFeatureViewModel>> RoleAndFeature { get; set; }
    }

    public class CreateRoleViewModel
    {
        public virtual ICollection<RoleAndFeatureViewModel> RoleAndFeatureViewModel { get; set; }
    }

    
}