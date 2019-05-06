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
    [Authorize(Roles = "Admin")]
    public class ReceiptsController : Controller
    {
        private FurnitureStoreEntities db = new FurnitureStoreEntities();

        // GET: Receipts
        public ActionResult Index(string option, string search)
        {
            List<ReceiptModel> receiptModels = new List<ReceiptModel>();

            foreach(var receipt in db.Receipts.ToList())
            {
                ReceiptModel newReceiptModel = new ReceiptModel();
                //var store = db.Stores.FirstOrDefault(x => x.StoreID == receipt.StoreID);

                newReceiptModel.ReceiptID = receipt.ReceiptID;
                newReceiptModel.Buyer = receipt.Buyer;
                newReceiptModel.Date_time = receipt.Date_time;
                newReceiptModel.Gross_price = receipt.Gross_price;
                newReceiptModel.Net_price = receipt.Net_price;
                //newReceiptModel.Store_Name = store.Store_Name;
                newReceiptModel.Taxes = receipt.Taxes;

                receiptModels.Add(newReceiptModel);
            }
          

            if (option == "Buyer")
            {
                return View(db.Receipts.Where(x => x.Buyer.ToUpper().Contains(search.ToUpper()) || search == null).ToList());
            }
            else if (option == "Store_Name")
            {
                return View(db.Stores.Where(x => x.Store_Name.ToUpper().Contains(search) || search == null).ToList());
            }
            else if (option == "Date")
            {
                return View(db.Receipts.Where(x => x.Date_time.ToShortDateString() == search || search == null).ToList());
            }
            
            return View(receiptModels);
        }

        // GET: Receipts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Receipt receipt = await db.Receipts.FindAsync(id);
            if (receipt == null)
            {
                return HttpNotFound();
            }
            return View(receipt);
        }

        // GET: Receipts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Receipts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ReceiptID,Net_price,Taxes,Gross_price,Date_time,Buyer")] Receipt receipt)
        {
            if (ModelState.IsValid)
            {
                db.Receipts.Add(receipt);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(receipt);
        }

        // GET: Receipts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Receipt receipt = await db.Receipts.FindAsync(id);
            if (receipt == null)
            {
                return HttpNotFound();
            }
            return View(receipt);
        }

        // POST: Receipts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ReceiptID,Net_price,Taxes,Gross_price,Date_time,Buyer")] Receipt receipt)
        {
            if (ModelState.IsValid)
            {
                db.Entry(receipt).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(receipt);
        }

        // GET: Receipts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Receipt receipt = await db.Receipts.FindAsync(id);
            if (receipt == null)
            {
                return HttpNotFound();
            }
            return View(receipt);
        }

        // POST: Receipts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Receipt receipt = await db.Receipts.FindAsync(id);
            db.Receipts.Remove(receipt);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
