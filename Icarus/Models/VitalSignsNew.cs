using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icarus.Models
{
    public class VitalSignsNew
    {
        public tblAdmission admission { get; set; }
        public IEnumerable<tblAdmissionVitalSign> vitalSignsLists { get; set; }
        public tblAdmissionVitalSign vitalSigns { get; set; }
    }
}