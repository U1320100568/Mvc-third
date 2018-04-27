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
            [Display(Name = "檔案用途類型")]
            public string AttachmentUse { get; set; }
            [Display(Name = "擁有人")]
            public Nullable<int> MemberId { get; set; }
            [Display(Name = "上傳時間")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0: yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "-")]
            public Nullable<System.DateTime> UploadTime { get; set; }
            [Display(Name = "最後下載時間")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0: yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "-")]
            public Nullable<System.DateTime> DownloadTime { get; set; }
            [Display(Name = "下載紀錄")]
            public virtual ICollection<AttachmentRecord> AttachmentRecord { get; set; }
        }

        
    }

    [MetadataType(typeof(AttachmentRecordMetadata))]
    public partial class AttachmentRecord
    {
        public class AttachmentRecordMetadata
        {
            
            [Display(Name = "下載紀錄")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0: yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "-")]
             public Nullable<System.DateTime> TimeDownload { get; set; }

        }


    }

}