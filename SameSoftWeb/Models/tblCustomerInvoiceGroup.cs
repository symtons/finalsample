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
    
    public partial class tblCustomerInvoiceGroup
    {
        public int Invoice_Group_Id { get; set; }
        public System.DateTime Invoice_Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> Customer_ID { get; set; }
        public Nullable<int> Invoice_Number { get; set; }
        public Nullable<System.DateTime> Last_Update { get; set; }
        public Nullable<int> Updated_by { get; set; }
    }
}