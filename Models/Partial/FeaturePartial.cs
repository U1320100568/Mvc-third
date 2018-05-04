using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Models
{
    [MetadataType(typeof(FeatureMetadata))]
    public partial class Feature
    {

        public class FeatureMetadata
        {
            public int FeatureId { get; set; }
            [Display(Name ="功能名稱")]
            public string FName { get; set; }
            [Display(Name = "controller名稱")]
            public string ControllerName { get; set; }
            [Display(Name = "action名稱")]
            public string ActionName { get; set; }
            [Display(Name = "更新人員")]
            public Nullable<int> UpdaterId { get; set; }
            [Display(Name = "更新日期")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString ="{0: yyyy/MM/dd HH:mm:ss}",ApplyFormatInEditMode =true,ConvertEmptyStringToNull =true,NullDisplayText = "-")]
            public Nullable<System.DateTime> UpdateDate { get; set; }
            public Nullable<bool> isDeleted { get; set; }
        }
    }
}