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
    
    public partial class tblDialyTask
    {
        public int Daily_Task_ID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string Task_Decription { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<System.DateTime> Task_Date { get; set; }
        public Nullable<System.TimeSpan> From_Time { get; set; }
        public Nullable<System.TimeSpan> To_Time { get; set; }
        public Nullable<System.DateTime> Record_Date_Time { get; set; }
        public Nullable<System.DateTime> Last_Updated { get; set; }
        public string Status { get; set; }
        public Nullable<float> Transportation { get; set; }
        public string Transportation_Mode { get; set; }
    
        public virtual tblCustomer tblCustomer { get; set; }
    }
}
