using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icarus.Models
{
    public class AttachmentNew
    {
        public tblAdmissionAttachment attachment { get; set; }
        public IEnumerable<tblAdmissionAttachment> attachmentLists { get; set; }
        public tblAdmission admission { get; set; }
        public IEnumerable<tblAdmissionAttachmentType> attachmentTypes { get; set; }
    }
}