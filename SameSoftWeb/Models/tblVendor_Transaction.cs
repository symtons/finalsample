//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SameSoftWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblVendor_Transaction
    {
        public int Vendor_Tran_ID { get; set; }
        public System.DateTime Tran_Date { get; set; }
        public float Amount { get; set; }
        public string Status { get; set; }
        public int UserID { get; set; }
        public int VendorID { get; set; }
    }
}
