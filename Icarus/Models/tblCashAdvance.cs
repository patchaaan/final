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
    
    public partial class tblCashAdvance
    {
        public int IDCA { get; set; }
        public System.DateTime DateCA { get; set; }
        public int IDStaff { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public string IsPaid { get; set; }
    }
}
