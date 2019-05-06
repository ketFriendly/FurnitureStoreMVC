using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FurnitureStore.Models
{
    public class ReceiptModel
    {
        public int ReceiptID { get; set; }
        public decimal Net_price { get; set; }
        public decimal Taxes { get; set; }
        public decimal Gross_price { get; set; }
        public System.DateTime Date_time { get; set; }
        public string Buyer { get; set; }
        //public string Store_Name { get; set; }
    }
}