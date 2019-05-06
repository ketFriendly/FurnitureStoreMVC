using FurnitureStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FurnitureStore.Controllers
{
    public class BrowsingController : Controller
    {
        private FurnitureStoreEntities db = new FurnitureStoreEntities();

        // GET: Browsing
        public ActionResult Index()
        {
            ViewBag.StoreID = new SelectList(db.Stores, "StoreID", "Store_Name");
            return View();
        }

        public ActionResult ReadOnlyList([Bind(Include = "StoreID")] string storeID)
        {
            return View(db.Furnitures.Where(x => x.Store.ToString() == storeID).ToList());
        }
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
    }
}