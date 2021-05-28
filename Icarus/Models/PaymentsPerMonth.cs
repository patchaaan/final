using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icarus.Models
{
    public class PaymentsPerMonth
    {
        public int Year { get; set; }
        public String Month { get; set; }
        public Double TotalPaid { get; set; }
    }
}