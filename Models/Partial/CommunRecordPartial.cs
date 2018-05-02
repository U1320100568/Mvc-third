using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Models
{
    [MetadataType(typeof(CommunRecordMetadata))]
    public partial class CommunRecord
    {
        public class CommunRecordMetadata
        {
            [Display(Name = "通聯記錄編號")]
            public int CPhoneRecordId { get; set; }
            [Display(Name = "檔案編號")]
            public int AttachmentId { get; set; }
            [Display(Name = "身份證字號")]
            public string IdCardNum { get; set; }
            [Display(Name = "目標電話")]
            public string CPhoneNum { get; set; }
            [Display(Name = "對象電話")]
            public string CCorrePhoneNum { get; set; }
            [Display(Name = "轉接")]
            public string CThroughPhoneNum { get; set; }
            [Display(Name = "通話類別")]
            public string CType { get; set; }
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0: yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "-")]
            [Display(Name = "始話日期時間")]
            public Nullable<System.DateTime> CStartTime { get; set; }
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0: yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "-")]
            [Display(Name = "結束日期時間")]
            public Nullable<System.DateTime> CEndTime { get; set; }
            [Display(Name = "IMEI")]
            public string CIMEI { get; set; }
            [Display(Name = "基地台編號")]
            public string CStationNum { get; set; }
            [Display(Name = "基地台位置")]
            public string CStationAddress { get; set; }
            
        }
    }
}