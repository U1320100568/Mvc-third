//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace IntelligenceCloud.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CommunRecord
    {
        public int CPhoneRecordId { get; set; }
        public int AttachmentId { get; set; }
        public string IdCardNum { get; set; }
        public string CPhoneNum { get; set; }
        public string CCorrePhoneNum { get; set; }
        public string CThroughPhoneNum { get; set; }
        public string CType { get; set; }
        public Nullable<System.DateTime> CStartTime { get; set; }
        public Nullable<System.DateTime> CEndTime { get; set; }
        public string CIMEI { get; set; }
        public string CStationNum { get; set; }
        public string CStationAddress { get; set; }
        public Nullable<bool> isDeleted { get; set; }
    }
}
