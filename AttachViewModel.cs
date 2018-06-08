using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Models
{
    public class AttachViewModel
    {
        public List<Attachment> AttachDetails { get; set; }
        public HttpPostedFileBase[] AttachFiles { get; set; }
        public string AttachmentUse { get; set; }
        public string ConnId { get; set; }
    }

    public class AttachSingleViewModel
    {
        public Attachment AttachDetail { get; set; }
        public AttachExcelViewModel AttachFile { get; set; }
        public string AttahcFilePath { get; set; }
    }
    public class AttachExcelViewModel
    {
        
        public List<AttachXmlRow> Table { get; set; }
        public AttachExcelViewModel()
        {
            Table = new List<AttachXmlRow>();
        }
    }
    public class AttachXmlRow
    {
        public List<string> ExcelRow { get; set;}
        public AttachXmlRow()
        {
            ExcelRow = new List<string>();
        }
    }

    public class AttachMemberShared
    {
        public Attachment Attach { get; set; }
        public List<Member> MemberShared { get; set; }
        public List<Member> MemberUnshared { get; set; }
    }
}