using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Models
{
    [MetadataType(typeof(AttachmentMetadata))]
    public partial class Attachment
    {
        public class AttachmentMetadata
        {
            [Display(Name = "檔案儲存路徑")]
            public string AttachmentPath { get; set; }
            [Display(Name = "檔案名稱")]
            public string AttachmentName { get; set; }
            [Display(Name = "檔案類型")]
            public string AttachmentType { get; set; }
            [Display(Name = "擁有人")]
            public Nullable<int> MemberId { get; set; }
            [Display(Name = "上傳時間")]
            public Nullable<System.DateTime> UploadTime { get; set; }
            [Display(Name = "最後下載時間")]
            public Nullable<System.DateTime> DownloadTime { get; set; }

            
        }

        
    }

    
}