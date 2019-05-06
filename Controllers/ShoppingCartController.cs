using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FurnitureStore.Models;

namespace FurnitureStore.Controllers
{
    [Authorize(Roles="Buyer")]
    public class ShoppingCartController : Controller
    {
        private FurnitureStoreEntities db = new FurnitureStoreEntities();
        private Receipt receiptGlobal = null;

        // GET: ShoppingCart
        public ActionResult Index()
        {
            Receipt receipt = (Receipt)HttpContext.Session["Receipt"];

            if (receipt != null)
            {
                receiptGlobal = receipt;
                return View(receiptGlobal.Items.ToList());
            }

            return View(new List<Item>());
        }

        public ActionResult AddToCart()
        {
            Furniture furniture = (Furniture)TempData["Furniture"];

            Receipt receipt = (Receipt)HttpContext.Session["Receipt"];

            if(receipt == null)
            {
                receipt = new Receipt();
            }

            Item item = receipt.Items.FirstOrDefault(i => i.Furniture == furniture.Furniture1);

            if(item != null)
            {
                item.Quantity++;
                item.Receipt.Net_price = item.Price_per_item * item.Quantity;
                item.Receipt.Taxes = item.Receipt.Net_price * 20 / 100;
                item.Receipt.Gross_price = item.Receipt.Net_price + item.Receipt.Taxes;
            }
            else
            {
                Item newItem = new Item();
                newItem.Furniture = furniture.Furniture1;
                newItem.Price_per_item = furniture.PricePerItem;
                newItem.Quantity = 1;
                newItem.Receipt = new Receipt();
                //newItem.Receipt.Store = furniture.Store1;
                newItem.StoreID = furniture.Store;
                newItem.Receipt.Buyer = User.Identity.Name;
                newItem.Receipt.Date_time = DateTime.Now;
                newItem.Receipt.Net_price = newItem.Price_per_item * newItem.Quantity;
                newItem.Receipt.Taxes = newItem.Receipt.Net_price * 20 / 100;
                newItem.Receipt.Gross_price = newItem.Receipt.Net_price + newItem.Receipt.Taxes;

                receipt.Items.Add(newItem);
            }

            HttpContext.Session["Receipt"] = receipt;

            receiptGlobal = receipt;

            return RedirectToAction("Index");
        }
        // GET: Furnitures/Delete/5
        public ActionResult Delete(string furniture)
        {
            if (String.IsNullOrEmpty(furniture))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Receipt receipt = (Receipt)HttpContext.Session["Receipt"];

            Item item = receipt.Items.FirstOrDefault(i => i.Furniture == furniture);

            if(item != null)
            {
                receipt.Items.Remove(item);
            }

            return RedirectToAction("Index");
        }

        public ActionResult FinishShopping()
        {
            Receipt receipt = (Receipt)HttpContext.Session["Receipt"];

            if (receipt != null)
            {
                receipt.Buyer = User.Identity.Name;
                receipt.Date_time = DateTime.Now;
                decimal totalNet = 0;
                decimal totalGross = 0;
                decimal totalTax = 0;
                foreach (Item item in receipt.Items)
                {
                    //item.receipt.storeId dodati listu receiptova po store id-u
                    totalNet = totalNet + item.Receipt.Net_price;
                    totalTax = totalTax + item.Receipt.Taxes;
                    totalGross = totalGross + item.Receipt.Gross_price;
                }
                receipt.Net_price = totalNet;
                receipt.Taxes = totalTax;
                receipt.Gross_price = totalGross;
                List<Item> items = receipt.Items.ToList();
                receipt.Items = null;
                var receiptDb = db.Receipts.Add(receipt);
                foreach (var item in items)
                {
                    item.Receipt = receiptDb;
                    item.ReceiptID = receiptDb.ReceiptID;
                    db.Items.Add(item);
                }
                
                db.SaveChanges();
                


                return View(receipt.Items.ToList());
            }

            return RedirectToAction("Index","Home");
        }
    }

}