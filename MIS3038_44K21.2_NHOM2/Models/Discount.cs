//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebBookStore.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Discount
    {
        public string id_Discount { get; set; }
        public string id_Book { get; set; }
        public string DiscountDetail { get; set; }
        public Nullable<System.DateTime> StaDate { get; set; }
        public Nullable<System.DateTime> ExpDate { get; set; }
    
        public virtual Book Book { get; set; }
    }
}
