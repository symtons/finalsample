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
    
    public partial class sp_get_vendor_transactions_by_id_Result
    {
        public int Vendor_Tran_ID { get; set; }
        public int Vendor_Transaction_Detail_id { get; set; }
        public int Expr1 { get; set; }
        public float Debit { get; set; }
        public float Credit { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        public System.DateTime Trans_Date { get; set; }
        public Nullable<int> Vendor_Tran_Type_ID { get; set; }
        public string Tran_Nbr { get; set; }
        public string Tran_Type { get; set; }
        public string FullName { get; set; }
        public int VendorID { get; set; }
        public string Vendor_Name { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
    }
}
