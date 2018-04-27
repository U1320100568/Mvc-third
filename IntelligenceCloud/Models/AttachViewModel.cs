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
    }

    public class AttachSingleViewModel
    {
        public Attachment AttachDetail { get; set; }
        public AttachXmlViewModel AttachFile { get; set; }
        public string AttahcFilePath { get; set; }
    }
    public class AttachXmlViewModel
    {
        
        public List<AttachXmlRow> Table { get; set; }
        public AttachXmlViewModel()
        {
            Table = new List<AttachXmlRow>();
        }
    }
    public class AttachXmlRow
    {
        public List<string> XmlRow { get; set;}
        public AttachXmlRow()
        {
            XmlRow = new List<string>();
        }
    }
}