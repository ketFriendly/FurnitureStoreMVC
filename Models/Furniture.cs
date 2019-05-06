//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FurnitureStore.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Furniture
    {
        public int FurnitureID { get; set; }
        public int ItemNumber { get; set; }
        public string Furniture1 { get; set; }
        public string CountryOfOrigin { get; set; }
        public System.DateTime ProductionYear { get; set; }
        public decimal PricePerItem { get; set; }
        public int Store { get; set; }
        public int TypeOfFurniture { get; set; }
        public string Image { get; set; }
    
        public virtual FurnitureType FurnitureType { get; set; }
        public virtual Store Store1 { get; set; }
    }
}