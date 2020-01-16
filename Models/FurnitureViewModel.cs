using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FurnitureStore.Models
{
    public class FurnitureViewModel
    {
        public int FurnitureID { get; set; }
        public int ItemNumber { get; set; }
        [Display(Name = "Furniture name")]
        [DataType(DataType.Text)]
        public string Furniture1 { get; set; }
        public string CountryOfOrigin { get; set; }
        public System.DateTime ProductionYear { get; set; }
        [DataType(DataType.Currency)]
        public decimal PricePerItem { get; set; }
        public int Store { get; set; }
        public int TypeOfFurniture { get; set; }
        public string Image { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public virtual FurnitureType FurnitureType { get; set; }
        public virtual Store Store1 { get; set; }
    }
}