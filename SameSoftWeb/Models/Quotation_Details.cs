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
    
    public partial class Quotation_Details
    {
        public int Quotation_detail_id { get; set; }
        public Nullable<int> Item_id { get; set; }
        public Nullable<float> Qty { get; set; }
        public Nullable<float> Price { get; set; }
        public int Quotation_ID { get; set; }
        public string Batch { get; set; }
    }
}
