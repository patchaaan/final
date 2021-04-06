using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icarus.Models
{
    public class AdmissionNew
    {
        public tblAdmission adm { get; set; }
        public tblRank rank { get; set; }    
        public IEnumerable<tblRank> rankList { get; set; }
    }
}