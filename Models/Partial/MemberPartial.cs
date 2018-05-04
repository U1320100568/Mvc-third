using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Models
{
    [MetadataType(typeof(MemberMetadata))]
    public partial class Member
    {
        public class MemberMetadata
        {
            
            [Display(Name = "姓名")]
            public string MemberName { get; set; }
            [Required]
            [Display(Name ="帳號")]
            public string MemberAccount { get; set; }
            [Required]
            [Display(Name = "密碼")]
            public string MemberPwd { get; set; }
            
            public Nullable<bool> isDeleted { get; set; }
            
        }
    }
}