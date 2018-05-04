using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Models
{
    [MetadataType(typeof(RoleMetaData))]
    public partial class Role
    {
        public class RoleMetaData
        {

            
            
            [Display(Name = "權限編號")]
            public int RoleId { get; set; }
            [Display(Name = "權限名稱")]
            public string RName { get; set; }
            [Display(Name = "更新日期")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString ="{0: yyyy/MM/dd HH:mm:ss}",ApplyFormatInEditMode =true,ConvertEmptyStringToNull =true , NullDisplayText ="-")]
            public Nullable<System.DateTime> UpdateDate { get; set; }
            [Display(Name = "更新人員")]
            public Nullable<int> UpdaterId { get; set; }
            [Display(Name = "權限鎖定")]
            public Nullable<bool> RLock { get; set; }
            
        }


    }

    [MetadataType(typeof(FeatureMetaData))]
    public partial class RoleFeature
    {
        public class FeatureMetaData
        {
            
        }
    }

    
    [MetadataType(typeof(RoleAndFeatureViewModelMetaData))]
    public partial class RoleAndFeatureViewModel
    {
        public class RoleAndFeatureViewModelMetaData
        {
            [Display(Name = "功能編號")]
            public int FeatureId { get; set; }
            [Display(Name = "Controller 名稱")]
            public string ControllerName { get; set; }
            [Display(Name = "Action 名稱")]
            [Required]
            public string ActionName { get; set; }
            [Display(Name = "權限鎖定")]
            public Nullable<bool> RoleLock { get; set; }
            [Display(Name = "權限編號")]
            public Nullable<int> RoleNum { get; set; }
            [Display(Name = "會員")]
            public int MemberId { get; set; }
            [Display(Name = "權限名稱")]
            public string Description { get; set; }
        }
    }
}