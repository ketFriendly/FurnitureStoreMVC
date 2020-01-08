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
    public class FurnitureTypesController : Controller
    {
        private FurnitureStoreEntities db = new FurnitureStoreEntities();
        
        // GET: FurnitureTypes
        public ActionResult Index( string search)
        {
            if(User.IsInRole("Admin"))
            { 
                if (search != null)
                {
                    return View(db.FurnitureTypes.Where(x => x.Name.ToUpper().Contains(search.ToUpper()) || search == null).ToList());
                }
                return View(db.FurnitureTypes.ToList());
            }
            else
            {
                return RedirectToAction("ReadOnlyList", "FurnitureTypes");
            }
            
        }
        [Authorize(Roles="Buyer")]
        public ActionResult ReadOnlyList(string search)
        {
            if (search != null)
            {
                return View(db.FurnitureTypes.Where(x => x.Name.Contains(search) || search == null).ToList());
            }
            return View(db.FurnitureTypes.ToList());
        }

        // GET: FurnitureTypes/Details/5
        [Authorize(Roles="Admin")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FurnitureType furnitureType = await db.FurnitureTypes.FindAsync(id);
            if (furnitureType == null)
            {
                return HttpNotFound();
            }
            return View(furnitureType);
        }

        // GET: FurnitureTypes/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: FurnitureTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CategoryID,Name,Description")] FurnitureType furnitureType)
        {
            if (ModelState.IsValid)
            {
                db.FurnitureTypes.Add(furnitureType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(furnitureType);
        }

        // GET: FurnitureTypes/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FurnitureType furnitureType = await db.FurnitureTypes.FindAsync(id);
            if (furnitureType == null)
            {
                return HttpNotFound();
            }
            return View(furnitureType);
        }

        // POST: FurnitureTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CategoryID,Name,Description")] FurnitureType furnitureType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(furnitureType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(furnitureType);
        }

        // GET: FurnitureTypes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FurnitureType furnitureType = await db.FurnitureTypes.FindAsync(id);
            if (furnitureType == null)
            {
                return HttpNotFound();
            }
            return View(furnitureType);
        }

        // POST: FurnitureTypes/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            FurnitureType furnitureType = await db.FurnitureTypes.FindAsync(id);
            db.FurnitureTypes.Remove(furnitureType);
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
