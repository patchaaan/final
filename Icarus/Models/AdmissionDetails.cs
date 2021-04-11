using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icarus.Models
{
    public class AdmissionDetails
    {
        public tblAdmission Admissions { get; set; }
        public tblAdmissionBilling AdmissionBilling { get; set; }
        public tblAssertion Assertion { get; set; }
        public tblAdmissionVitalSign VitalSigns { get; set; }
        public tblAdmissionCommLog CommLog { get; set; }
        public AttachmentNew Attachments { get; set; }
        public IEnumerable<tblPayment> Payments { get; set; }
    }
}