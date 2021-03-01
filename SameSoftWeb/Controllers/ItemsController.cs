using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SameSoftWeb.Models;

namespace SameSoftWeb.Controllers
{
    public class ItemsController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: Items
        public ActionResult Index()
        {

            object divid = 1;
       
 int id = (int)divid;
 //var items = db.Items.Include(i => i.Account).Include(i => i.Account1).Include(i => i.Account2).Include(i => i.Account3).Where(a=>a.Business_Division_Id== id);
            return View();
        }

        // GET: Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Items/Create
        public ActionResult Create()
        {
          

            ViewBag.catg = new SelectList(db.Categories, "Category_Id", "Name");
            ViewBag.Measurement_Id = new SelectList(db.Measurements, "Measurement_Id", "Code");
            
            ViewBag.manufacturer = new SelectList(db.Manufacturers, "ManufacturerId", "Name");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Item_Id,Category_Id,Measurement_Id,SalesAccountId,InventoryAccountId,CostOfGoodsSoldAccountId,InventoryAdjustmentAccountId,Bar_Code,Name,Unit_Cost,Sale_Price,Re_Order_Leval,IsActive,ManufacturerId,Business_Division_Id,ReferenceNo,Description")] Item item)
        {
            if (ModelState.IsValid)
            {
               db.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

       
            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Name");
            ViewBag.ManufacturerId = new SelectList(db.Manufacturers, "ManufacturerId", "Name");
            ViewBag.Measurement_Id = new SelectList(db.Measurements, "Measurement_Id", "Code");
            return View(item);
        }

        // GET: Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
    

            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Name");
            ViewBag.ManufacturerId = new SelectList(db.Manufacturers, "ManufacturerId", "Name");
            ViewBag.Measurement_Id = new SelectList(db.Measurements, "Measurement_Id", "Code");
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Item_Id,Category_Id,Measurement_Id,SalesAccountId,InventoryAccountId,CostOfGoodsSoldAccountId,InventoryAdjustmentAccountId,Bar_Code,Name,Unit_Cost,Sale_Price,Re_Order_Leval,IsActive,ManufacturerId,ReferenceNo,Description,Business_Division_Id")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        

            ViewBag.Measurement_Id = new SelectList(db.Measurements, "Measurement_Id", "Code");

            ViewBag.Category_Id = new SelectList(db.Categories, "Category_Id", "Name");

            return View(item);
        }

        // GET: Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
            db.SaveChanges();
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
