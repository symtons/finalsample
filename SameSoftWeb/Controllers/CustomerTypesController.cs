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
    public class CustomerTypesController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: CustomerTypes
        public ActionResult Index()
        {
            return View(db.tblCustomerTypes.ToList());
        }

        // GET: CustomerTypes/Details/6
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomerType tblCustomerType = db.tblCustomerTypes.Find(id);
            if (tblCustomerType == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomerType);
        }

        // GET: CustomerTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Customer_Type")] tblCustomerType tblCustomerType)
        {
            if (ModelState.IsValid)
            {
                db.tblCustomerTypes.Add(tblCustomerType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblCustomerType);
        }

        // GET: CustomerTypes/Edit/6
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomerType tblCustomerType = db.tblCustomerTypes.Find(id);
            if (tblCustomerType == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomerType);
        }

        // POST: CustomerTypes/Edit/6
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Customer_Type")] tblCustomerType tblCustomerType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblCustomerType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblCustomerType);
        }

        // GET: CustomerTypes/Delete/6
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomerType tblCustomerType = db.tblCustomerTypes.Find(id);
            if (tblCustomerType == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomerType);
        }

        // POST: CustomerTypes/Delete/6
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblCustomerType tblCustomerType = db.tblCustomerTypes.Find(id);
            db.tblCustomerTypes.Remove(tblCustomerType);
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
