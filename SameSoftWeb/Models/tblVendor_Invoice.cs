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
    
    public partial class tblVendor_Invoice
    {
        public int Invoice_Nbr { get; set; }
        public System.DateTime Invoice_Date { get; set; }
        public Nullable<System.DateTime> Payment_Date { get; set; }
        public string Payment_Method { get; set; }
        public int UserID { get; set; }
        public string Check_Number { get; set; }
        public string Received_by { get; set; }
        public string Status { get; set; }
        public Nullable<float> Amount { get; set; }
        public int VendorID { get; set; }
        public string Payment_type { get; set; }
    }
}
