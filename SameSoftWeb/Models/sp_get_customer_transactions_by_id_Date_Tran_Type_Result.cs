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
    
    public partial class sp_get_customer_transactions_by_id_Date_Tran_Type_Result
    {
        public int CustomerID { get; set; }
        public string Customer_Name { get; set; }
        public string Contact_Numbers { get; set; }
        public string Address { get; set; }
        public int Cust_Tran_ID { get; set; }
        public int Cust_Transaction_Detail_id { get; set; }
        public float Debit { get; set; }
        public float Credit { get; set; }
        public string Description { get; set; }
        public System.DateTime Trans_Date { get; set; }
        public string Tran_Type { get; set; }
        public int UserID { get; set; }
        public string FullName { get; set; }
        public int Cust_Tran_Type_ID { get; set; }
        public Nullable<float> Qty { get; set; }
        public Nullable<float> Unit_price { get; set; }
        public string Status { get; set; }
        public Nullable<int> Invoice_No { get; set; }
    }
}