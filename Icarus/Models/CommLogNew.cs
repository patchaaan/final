using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icarus.Models
{
    public class CommLogNew
    {
        public tblAdmission admission { get; set; }
        public IEnumerable<tblAdmissionCommLog> commLogLists { get; set; }
        public tblAdmissionCommLog commLog { get; set; }
    }
}