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
    
    public partial class tblQuotation
    {
        public int Quotation_ID { get; set; }
        public System.DateTime Tran_Date { get; set; }
        public string Tran_Nbr { get; set; }
        public float Total { get; set; }
        public string Status { get; set; }
        public int UserID { get; set; }
        public int Business_Division_Id { get; set; }
        public float Discount { get; set; }
        public Nullable<int> Customer_ID { get; set; }
        public Nullable<int> Marketer_id { get; set; }
    }
}
