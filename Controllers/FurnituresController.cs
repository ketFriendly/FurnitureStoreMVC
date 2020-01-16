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
using System.IO;
using System.Configuration;
using System.Net.Http;
using System.Web.Helpers;

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
                    return View(db.Furnitures.Where(x => x.Furniture1.ToUpper().Contains(search.ToUpper()) || search == "").ToList());
                }
                else if (option == "CountryOfOrigin")
                {
                    return View(db.Furnitures.Where(x => x.CountryOfOrigin.ToUpper().Contains(search.ToUpper()) || search == "").ToList());
                }
                else if (option == "ProductionYear")
                {
                    return View(db.Furnitures.Where(x => x.ProductionYear.ToString().Contains(search) || search == "").ToList());
                }
                else if (option == "PricePerItem")
                {
                    return View(db.Furnitures.Where(x => x.PricePerItem.ToString().Contains(search) || search == "").ToList());
                }
                var furnitures = db.Furnitures.Include(f => f.FurnitureType).Include(f => f.Store1);
                return View(furnitures.ToList());
            }
            else
            {
                return RedirectToAction("ReadOnlyList", "Furnitures");
            }
        }

        public ActionResult ReadOnlyList(string option, string search)
        {
            if (option == "Furniture1")
            {
                return View(db.Furnitures.Where(x => x.Furniture1.Contains(search) || search == "").ToList());
            }
            else if (option == "CountryOfOrigin")
            {
                return View(db.Furnitures.Where(x => x.CountryOfOrigin.Contains(search) || search == "").ToList());
            }
            else if (option == "ProductionYear")
            {
                return View(db.Furnitures.Where(x => x.ProductionYear.ToString().Contains(search) || search == "").ToList());
            }
            else if (option == "PricePerItem")
            {
                return View(db.Furnitures.Where(x => x.PricePerItem.ToString().Contains(search) || search == "").ToList());
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
        public async Task<ActionResult> Create([Bind(Include = "FurnitureID,ItemNumber,Furniture1,CountryOfOrigin,ProductionYear,PricePerItem,Store,TypeOfFurniture,Image,ImageFile")]FurnitureViewModel furniture)
        {
            WebImage photo = null;
            photo = WebImage.GetImageFromRequest();
            
            
            if (ModelState.IsValid)
            {
                
                var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };
                string FileName = Path.GetFileNameWithoutExtension(photo.FileName);
                string FileExtension = Path.GetExtension(photo.FileName);
                var allFurniture = db.Furnitures.ToList();
                var lastEntry = allFurniture.Last();
                int lastNum = lastEntry.ItemNumber;
                if(allowedExtensions.Contains(FileExtension))
                {
                    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;
                    
                    var path = Path.Combine(Server.MapPath("~/Content"), FileName);
                    photo.Save(path);
                    
                    Furniture furnitureObj = new Furniture();
                    furnitureObj.CountryOfOrigin = furniture.CountryOfOrigin;
                    furnitureObj.ItemNumber = lastNum+1;
                    furnitureObj.Furniture1 = furniture.Furniture1;
                    furnitureObj.FurnitureType = db.FurnitureTypes.FirstOrDefault(x => x.CategoryID == furniture.TypeOfFurniture);
                    furnitureObj.ProductionYear = furniture.ProductionYear;
                    furnitureObj.PricePerItem = furniture.PricePerItem;
                    furnitureObj.Image = "/Content/" + FileName;
                    furnitureObj.Store1 = db.Stores.FirstOrDefault(x => x.StoreID == furniture.Store);
                    furnitureObj.Store = furniture.Store;
                    furnitureObj.TypeOfFurniture = furniture.TypeOfFurniture;
                    db.Furnitures.Add(furnitureObj);
                    
                    await db.SaveChangesAsync();
                    //if (!System.IO.File.Exists(path))
                    //        System.IO.File.Create(path);
                    
                }
                else
                {
                    ViewBag.message = "Please choose .Jpg, .png, .jpg or .jpeg files";
                }
               
            }

            //ViewBag.TypeOfFurniture = new SelectList(db.FurnitureTypes, "CategoryID", "Name", furniture.TypeOfFurniture);
            //ViewBag.Store = new SelectList(db.Stores, "StoreID", "Store_Name", furniture.Store);
            return RedirectToAction("Index");
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

            FurnitureViewModel furnitureVM = furnitureModelToFurniture(furniture, null);
            //var path = Path.Combine(Server.MapPath("~/"), furniture.Image);
            if (furniture == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypeOfFurniture = new SelectList(db.FurnitureTypes, "CategoryID", "Name", furniture.TypeOfFurniture);
            ViewBag.Store = new SelectList(db.Stores, "StoreID", "Store_Name", furniture.Store);
            return View(furnitureVM);
        }

        // POST: Furnitures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit([Bind(Include = "FurnitureID,ItemNumber,Furniture1,CountryOfOrigin,ProductionYear,PricePerItem,Store,TypeOfFurniture,Image,ImageFile")] FurnitureViewModel editedFurnitureVM)
        {
            var idFromRoute = Url.RequestContext.RouteData.Values["id"].ToString();
            int id = Int32.Parse(idFromRoute);
            Furniture editedFurniture = furnitureModelToFurniture(editedFurnitureVM);
            Furniture originalFurniture = db.Furnitures.AsNoTracking().FirstOrDefault(x => x.FurnitureID == id);
            originalFurniture.Store1 = null;
            originalFurniture.FurnitureType = null;
            
            editedFurniture.FurnitureID = originalFurniture.FurnitureID;
            editedFurniture.ItemNumber = originalFurniture.ItemNumber;
            
            WebImage photo = null;
            photo = WebImage.GetImageFromRequest();
            string FileName = "";
            string FileExtension = "";
            if (photo != null)
            {
                FileName = Path.GetFileName(photo.FileName);
                FileExtension = Path.GetExtension(photo.FileName);
            }
            
            
            var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };
            var state = db.Entry(originalFurniture).State;
            if (ModelState.IsValid)
            {
                if (!originalFurniture.Equals(editedFurniture))
                {
                    if ( originalFurniture.Image!= "/Content/" + FileName)
                    {
                        if(FileName!="" && allowedExtensions.Contains(FileExtension))
                        {
                            FileName = Guid.NewGuid().ToString() + "-" + FileName.Trim();

                            var path = Path.Combine(Server.MapPath("~/Content"), FileName);
                            photo.Save(path);

                            originalFurniture.Image = "/Content/" + FileName.Trim();
                        }
                    }
                    if (originalFurniture.TypeOfFurniture != editedFurniture.TypeOfFurniture)
                    {
                        originalFurniture.TypeOfFurniture = editedFurniture.TypeOfFurniture;
                        originalFurniture.FurnitureType = null;
                    }
                    if (originalFurniture.Store != editedFurniture.Store)
                    {
                        originalFurniture.Store = editedFurniture.Store;
                        originalFurniture.Store1 = null;
                    }
                    originalFurniture.CountryOfOrigin = editedFurniture.CountryOfOrigin;
                    originalFurniture.PricePerItem = editedFurniture.PricePerItem;
                    originalFurniture.ProductionYear = editedFurniture.ProductionYear;
                }

                db.Entry(originalFurniture).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TypeOfFurniture = new SelectList(db.FurnitureTypes, "CategoryID", "Name", editedFurnitureVM.TypeOfFurniture);
            ViewBag.Store = new SelectList(db.Stores, "StoreID", "Store_Name", editedFurnitureVM.Store);
            return View(editedFurnitureVM);
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
            var filePath = Server.MapPath("~" + furniture.Image);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
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
        protected Furniture furnitureModelToFurniture(FurnitureViewModel furnitureVM)
        {
            Furniture furniture = new Furniture();
            furniture.CountryOfOrigin = furnitureVM.CountryOfOrigin;
            furniture.ItemNumber = furnitureVM.ItemNumber;
            furniture.Furniture1 = furnitureVM.Furniture1;
            //furniture.FurnitureType = db.FurnitureTypes.FirstOrDefault(x => x.CategoryID == furnitureVM.TypeOfFurniture);
            furniture.ProductionYear = furnitureVM.ProductionYear;
            furniture.PricePerItem = furnitureVM.PricePerItem;
            furniture.Image = furnitureVM.Image;
            //furniture.Store1 = db.Stores.FirstOrDefault(x => x.StoreID  == furnitureVM.Store);
            furniture.Store = furnitureVM.Store;
            furniture.TypeOfFurniture = furnitureVM.TypeOfFurniture;
            return furniture;
        }
        protected FurnitureViewModel furnitureModelToFurniture(Furniture Furniture, HttpPostedFileBase image)
        {

            FurnitureViewModel furniture = new FurnitureViewModel();
            furniture.CountryOfOrigin = Furniture.CountryOfOrigin;
            furniture.ItemNumber = Furniture.ItemNumber;
            furniture.Furniture1 = Furniture.Furniture1;
            //furniture.FurnitureType = Furniture.FurnitureType;
            furniture.ProductionYear = Furniture.ProductionYear;
            furniture.PricePerItem = Furniture.PricePerItem;
            furniture.Image = Furniture.Image;
            //furniture.Store1 = Furniture.Store1;
            furniture.Store = Furniture.Store;
            furniture.TypeOfFurniture = Furniture.TypeOfFurniture;
            furniture.ImageFile = image;
            return furniture;
        }
    } 
}
