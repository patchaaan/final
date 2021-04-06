using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icarus.Models
{
    public class AssertionNew
    {
        public tblAdmission admission { get; set; }
        public IEnumerable<tblAssertion> assertionLists { get; set; }
        public tblAssertion assertion { get; set; }
    }
}