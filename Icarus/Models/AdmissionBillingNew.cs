using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icarus.Models
{
    public class AdmissionBillingNew
    {
        public tblAdmissionBilling admBilling { get; set; }
        public IEnumerable<tblAdmissionBilling> admBillingList { get; set; }
        public tblAdmission admissionData { get; set; }
    }
}