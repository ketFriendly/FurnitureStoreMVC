using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FurnitureStore.Models;

namespace FurnitureStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        FurnitureStoreEntities db = new FurnitureStoreEntities();
        // GET: Reports
        public ActionResult Index([Bind(Include = "FurnitureTypeID,StartDate,EndDate")] string furnitureTypeID, string startdate, string enddate)
        {
            List<ReportModel> reportsModel = new List<ReportModel>();
            decimal total = 0;

            //validate
            if(string.IsNullOrEmpty(startdate) || string.IsNullOrEmpty(enddate))
            {
                ViewBag.TotalReportSum = total;
                ViewBag.FurnitureTypeID = new SelectList(db.FurnitureTypes, "CategoryID", "Name");
                return View(reportsModel);
            }
            DateTime startDate = DateTime.Parse(startdate);
            DateTime endDate = DateTime.Parse(enddate);

            List<Receipt> receiptsForDesiredPeriod = db.Receipts.Where(x => x.Date_time >= startDate && x.Date_time <= endDate).ToList();

            foreach (Receipt receipt in receiptsForDesiredPeriod)
            {
                ReportModel reportModel = new ReportModel();
                reportModel.Date = receipt.Date_time;
                // add store name:
                
                //reportModel.StoreName = db.Stores.FirstOrDefault(x => x.StoreID == receipt.Items.StoreID).Store_Name;

                // add number of sold furniture for desired category:
                List<Item> items = db.Items.Where(x => x.ReceiptID == receipt.ReceiptID).ToList();
                foreach (Item item in items)
                {
                    if (db.Furnitures.FirstOrDefault(x => x.Furniture1 == item.Furniture).FurnitureType.CategoryID.ToString() == furnitureTypeID)
                    {
                        reportModel.FurnitureType = furnitureTypeID;
                        reportModel.Sold += item.Quantity;
                        reportModel.SumForType += item.Price_per_item * item.Quantity;
                    }

                }
                for (int i = 0; i < items.Count; i++)
                {
                    //if (db.Furnitures.FirstOrDefault(x => x.Furniture1 == items[i].Furniture).FurnitureType.CategoryID.ToString() == furnitureTypeID)
                    //{
                    //    reportModel.FurnitureType = furnitureTypeID;
                    //    reportModel.Sold += items[i].Quantity;
                    //    reportModel.SumForType += items[i].Price_per_item * items[i].Quantity;
                    //}
                    int storeId = items[i].StoreID;
                    string storeName = db.Stores.FirstOrDefault(x => x.StoreID == storeId).Store_Name;
                    if (reportModel.StoreName.Count != 0)
                    {
                        if (!reportModel.StoreName.Contains(storeName))
                        {
                            reportModel.StoreName.Add(storeName);
                        }
                    }
                    else
                    {
                        reportModel.StoreName.Add(storeName);
                    }
                    
                }
                

                reportsModel.Add(reportModel);
                total += reportModel.SumForType;
            }

            ViewBag.TotalReportSum = total;
            ViewBag.FurnitureTypeID = new SelectList(db.FurnitureTypes, "CategoryID", "Name");
            return View(reportsModel);
        }
        
    }
}