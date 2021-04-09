using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icarus.Models
{
    public class PaymentHistoryNew
    {
        public tblAdmission admission { get; set; }
        public IEnumerable<tblPayment> paymentLists {get; set;}
        public tblPayment payments { get; set; }
    }
}