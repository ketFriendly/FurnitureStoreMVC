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
    [Authorize]
    public class StoresController : Controller
    {
        private FurnitureStoreEntities db = new FurnitureStoreEntities();

        // GET: Stores
        public ActionResult Index(string option, string search)
        {
            if (User.IsInRole("Admin"))
            {
                if (option == "Store_Name")
                {
                    return View(db.Stores.Where(x => x.Store_Name.ToUpper().Contains(search.ToUpper()) || search == null).ToList());
                }
                else if (option == "Owner")
                {
                    return View(db.Stores.Where(x => x.Owner.ToUpper().Contains(search.ToUpper()) || search == null).ToList());
                }
                else if (option == "Address")
                {
                    return View(db.Stores.Where(x => x.Address.ToUpper().Contains(search.ToUpper()) || search == null).ToList());
                }
                else if (option == "TIN")
                {
                    return View(db.Stores.Where(x => x.TIN.ToString().Contains(search) || search == null).ToList());
                }
                else if (option == "BankAccountNumber")
                {
                    return View(db.Stores.Where(x => x.BankAccountNumber.ToUpper().Contains(search.ToUpper()) || search == null).ToList());
                }
                return View(db.Stores.ToList());
            }
            else
            {
                return RedirectToAction ("ReadOnlyList");
            }
        }
        public ActionResult ReadOnlyList(string option, string search)
        {
            if (option == "Store_Name")
            {
                return View(db.Stores.Where(x => x.Store_Name.Contains(search) || search == null).ToList());
            }
            else if (option == "Owner")
            {
                return View(db.Stores.Where(x => x.Owner.Contains(search) || search == null).ToList());
            }
            else if (option == "Address")
            {
                return View(db.Stores.Where(x => x.Address.Contains(search) || search == null).ToList());
            }
            else if (option == "TIN")
            {
                return View(db.Stores.Where(x => x.TIN.ToString().Contains(search) || search == null).ToList());
            }
            else if (option == "BankAccountNumber")
            {
                return View(db.Stores.Where(x => x.BankAccountNumber.Contains(search) || search == null).ToList());
            }
            return View(db.Stores.ToList());
        }
        // GET: Stores/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = await db.Stores.FindAsync(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // GET: Stores/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Store_Name,Owner,Address,Phone,Email,Website,TIN,BankAccountNumber")] Store store)
        {
            if (ModelState.IsValid)
            {
                db.Stores.Add(store);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(store);
        }

        // GET: Stores/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = await db.Stores.FindAsync(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "StoreID, Store_Name,Owner,Address,Phone,Email,Website,TIN,BankAccountNumber")] Store store)
        {
            if (ModelState.IsValid)
            {
                db.Entry(store).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(store);
        }

        // GET: Stores/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = await db.Stores.FindAsync(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Store store = await db.Stores.FindAsync(id);
            db.Stores.Remove(store);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
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
