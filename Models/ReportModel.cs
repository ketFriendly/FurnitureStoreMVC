using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FurnitureStore.Models
{
    
    public class ReportModel
    {
        public ReportModel()
        {
            StoreName = new List<string>();
        }
        public string FurnitureType { get; set; }
        public List<string> StoreName { get; set; }
        public int Sold { get; set; }
        public decimal SumForType { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}