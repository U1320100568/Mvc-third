using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Models
{
    [MetadataType(typeof( RoleMataData))]
    public partial class Role
    {
        public class RoleMataData
        {
            [Display(Name = "管理員權限")]
            public Nullable<bool> RoleAdmin { get; set; }
            [Display(Name = "會員權限")]
            public Nullable<bool> RoleNormal { get; set; }
            [Display(Name = "權限鎖定")]
            public Nullable<bool> RoleLock { get; set; }
            
        }
    }
}