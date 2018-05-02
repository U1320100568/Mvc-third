using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Models.Partial
{
    [MetadataType(typeof(ForensicContactMetaData))]
    public partial class ForensicContact
    {
        

        public class ForensicContactMetaData
        {
            [Display(Name ="鑑識報告連路人ID")]
            public int ForensicContactID { get; set; }
            [Display(Name = "檔案ID")]
            public Nullable<int> AttachmentId { get; set; }
            [Display(Name = "分頁號碼")]
            public Nullable<int> FSheetNum { get; set; }
            [Display(Name = "聯絡人")]
            public string FSheetName { get; set; }
            [Display(Name = "#")]
            public Nullable<int> FNum { get; set; }
            [Display(Name = "名稱")]
            public string FName { get; set; }
            [Display(Name = "群組")]
            public string FGroup { get; set; }
            [Display(Name = "已修改時間")]
            public Nullable<System.DateTime> FModifiedTime { get; set; }
            [Display(Name = "條目")]
            public string FClause { get; set; }
            [Display(Name = "備註")]
            public string FNote { get; set; }
            [Display(Name = "來源")]
            public string FSource { get; set; }
            [Display(Name = "已刪除")]
            public Nullable<bool> isDeleted { get; set; }
            
        }
    }

    [MetadataType(typeof(ForensicSMSMetadata))]
    public partial class ForensicSMS
    {
        

        public class ForensicSMSMetadata
        {
            [Display(Name = "鑑識報告簡訊ID")]
            public int ForensicSMSID { get; set; }
            [Display(Name = "檔案ID")]
            public Nullable<int> AttachmentId { get; set; }
            [Display(Name = "分頁號碼")]
            public Nullable<int> FSheetNum { get; set; }
            [Display(Name = "SMS訊息")]
            public string FSheetName { get; set; }
            [Display(Name = "#")]
            public Nullable<int> FNum { get; set; }
            [Display(Name = "合作對象")]
            public string FCorrespond { get; set; }
            [Display(Name = "時間")]
            public Nullable<System.DateTime> FDatetime { get; set; }
        }
    }
}