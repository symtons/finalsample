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
    
    public partial class tblTask
    {
        public int TaskID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> Creation_Date { get; set; }
        public Nullable<int> Project_id { get; set; }
        public string Priority { get; set; }
        public Nullable<int> Assigned_for { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> Last_Update { get; set; }
        public string Start_Date { get; set; }
        public string End_Date { get; set; }
        public Nullable<int> Created_by { get; set; }
        public string Notes { get; set; }
        public Nullable<int> Customer_id { get; set; }
        public string Department { get; set; }
    }
}
