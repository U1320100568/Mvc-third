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
    }
}