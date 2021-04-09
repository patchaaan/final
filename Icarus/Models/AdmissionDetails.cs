using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icarus.Models
{
    public class AdmissionDetails
    {
        public AdmissionNew Admissions { get; set; }
        public AdmissionBillingNew admissiongBillingNew { get; set; }
        public AssertionNew Assertion { get; set; }
        public VitalSignsNew VitalSigns { get; set; }
        public CommLogNew CommLog { get; set; }
        public AttachmentNew Attachments { get; set; }
        public PaymentHistoryNew Payments { get; set; }
        public tblRank Rank { get; set; }
        public IEnumerable<tblRank> rankLists { get; set; }
    }
}