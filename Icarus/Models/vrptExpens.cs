//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Icarus.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class vrptExpens
    {
        public int IDExpense { get; set; }
        public Nullable<System.DateTime> ExpenseDate { get; set; }
        public string ORNumber { get; set; }
        public string Vendor { get; set; }
        public string Particulars { get; set; }
        public string Account { get; set; }
        public string EncodedBy { get; set; }
        public string ChargeToCodep { get; set; }
        public Nullable<decimal> VATSales { get; set; }
        public Nullable<decimal> VATAmount { get; set; }
        public Nullable<decimal> VATExempt { get; set; }
        public decimal Amount { get; set; }
    }
}
