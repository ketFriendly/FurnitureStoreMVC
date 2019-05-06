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
    public class FurnituresController : Controller
    {
        private FurnitureStoreEntities db = new FurnitureStoreEntities();

        // GET: Furnitures
 
        public ActionResult Index(string option, string search)
        {
            if (User.IsInRole("Admin"))
            {
                if (option == "Furniture1")
                {
                    return View(db.Furnitures.Where(x => x.Furniture1.ToUpper().Contains(search.ToUpper()) || search == null).ToList());
                }
                else if (option == "CountryOfOrigin")
                {
                    return View(db.Furnitures.Where(x => x.CountryOfOrigin.ToUpper().Contains(search.ToUpper()) || search == null).ToList());
                }
                else if (option == "ProductionYear")
                {
                    return View(db.Furnitures.Where(x => x.ProductionYear.ToString().Contains(search) || search == null).ToList());
                }
                else if (option == "PricePerItem")
                {
                    return View(db.Furnitures.Where(x => x.PricePerItem.ToString().Contains(search) || search == null).ToList());
                }
                var furnitures = db.Furnitures.Include(f => f.FurnitureType).Include(f => f.Store1);
                return View(furnitures.ToList());
            }
            else 
            {
                return RedirectToAction("ReadOnlyList","Furnitures");
            }
        }
        
        public ActionResult ReadOnlyList(string option, string search)
        {
            if (option == "Furniture1")
            {
                return View(db.Furnitures.Where(x => x.Furniture1.Contains(search) || search == null).ToList());
            }
            else if (option == "CountryOfOrigin")
            {
                return View(db.Furnitures.Where(x => x.CountryOfOrigin.Contains(search) || search == null).ToList());
            }
            else if (option == "ProductionYear")
            {
                return View(db.Furnitures.Where(x => x.ProductionYear.ToString().Contains(search) || search == null).ToList());
            }
            else if (option == "PricePerItem")
            {
                return View(db.Furnitures.Where(x => x.PricePerItem.ToString().Contains(search) || search == null).ToList());
            }

            var furnitures = db.Furnitures.Include(f => f.FurnitureType).Include(f => f.Store1);
            return View(furnitures.ToList());
        }
        // GET: Furnitures/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Furniture furniture = await db.Furnitures.FindAsync(id);
            if (furniture == null)
            {
                return HttpNotFound();
            }
            return View(furniture);
        }

        // GET: Furnitures/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.TypeOfFurniture = new SelectList(db.FurnitureTypes, "CategoryID", "Name");
            ViewBag.Store = new SelectList(db.Stores, "StoreID", "Store_Name");
            return View();
        }

        // POST: Furnitures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([Bind(Include = "FurnitureID,ItemNumber,Furniture1,CountryOfOrigin,ProductionYear,PricePerItem,Store,TypeOfFurniture,Image")] Furniture furniture)
        {
            if (ModelState.IsValid)
            {
                furniture.Image = "/Content/" + furniture.Image;
                db.Furnitures.Add(furniture);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TypeOfFurniture = new SelectList(db.FurnitureTypes, "CategoryID", "Name", furniture.TypeOfFurniture);
            ViewBag.Store = new SelectList(db.Stores, "StoreID", "Store_Name", furniture.Store);
            return View(furniture);
        }

        // GET: Furnitures/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Furniture furniture = await db.Furnitures.FindAsync(id);
            if (furniture == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypeOfFurniture = new SelectList(db.FurnitureTypes, "CategoryID", "Name", furniture.TypeOfFurniture);
            ViewBag.Store = new SelectList(db.Stores, "StoreID", "Store_Name", furniture.Store);
            return View(furniture);
        }

        // POST: Furnitures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit([Bind(Include = "FurnitureID,ItemNumber,Furniture1,CountryOfOrigin,ProductionYear,PricePerItem,Store,TypeOfFurniture,Image")] Furniture furniture)
        {
            if (ModelState.IsValid)
            {
                db.Entry(furniture).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TypeOfFurniture = new SelectList(db.FurnitureTypes, "CategoryID", "Name", furniture.TypeOfFurniture);
            ViewBag.Store = new SelectList(db.Stores, "StoreID", "Store_Name", furniture.Store);
            return View(furniture);
        }
        // GET: Furnitures/AddToCart/5
       
        public async Task<ActionResult> AddToCart(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Furniture furniture = await db.Furnitures.FindAsync(id);
            if (furniture == null)
            {
                return HttpNotFound();
            }

            TempData["Furniture"] = furniture;

            return RedirectToAction("AddToCart", "ShoppingCart", TempData["Furniture"]);
        }

        // POST: Furnitures/AddToCart/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddToCart([Bind(Include = "FurnitureID,ItemNumber,Furniture1,CountryOfOrigin,ProductionYear,PricePerItem,Store,TypeOfFurniture,Image")] Furniture furniture)
        {
            if (ModelState.IsValid)
            {
                db.Entry(furniture).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TypeOfFurniture = new SelectList(db.FurnitureTypes, "CategoryID", "Name", furniture.TypeOfFurniture);
            ViewBag.Store = new SelectList(db.Stores, "StoreID", "Store_Name", furniture.Store);
            return View(furniture);
        }

        // GET: Furnitures/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Furniture furniture = await db.Furnitures.FindAsync(id);
            if (furniture == null)
            {
                return HttpNotFound();
            }
            return View(furniture);
        }

        // POST: Furnitures/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Furniture furniture = await db.Furnitures.FindAsync(id);
            db.Furnitures.Remove(furniture);
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
