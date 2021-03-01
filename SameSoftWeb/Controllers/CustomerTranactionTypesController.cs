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
    public class CustomerTranactionTypesController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: CustomerTranactionTypes
        public ActionResult Index()
        {
            return View(db.tblCustomerTranactionTypes.ToList());
        }

        // GET: CustomerTranactionTypes/Details/6
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomerTranactionType tblCustomerTranactionType = db.tblCustomerTranactionTypes.Find(id);
            if (tblCustomerTranactionType == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomerTranactionType);
        }

        // GET: CustomerTranactionTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerTranactionTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Transaction_type_id,Transaction_type")] tblCustomerTranactionType tblCustomerTranactionType)
        {
            if (ModelState.IsValid)
            {
                db.tblCustomerTranactionTypes.Add(tblCustomerTranactionType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblCustomerTranactionType);
        }

        // GET: CustomerTranactionTypes/Edit/6
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomerTranactionType tblCustomerTranactionType = db.tblCustomerTranactionTypes.Find(id);
            if (tblCustomerTranactionType == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomerTranactionType);
        }

        // POST: CustomerTranactionTypes/Edit/6
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Transaction_type_id,Transaction_type")] tblCustomerTranactionType tblCustomerTranactionType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblCustomerTranactionType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblCustomerTranactionType);
        }

        // GET: CustomerTranactionTypes/Delete/6
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomerTranactionType tblCustomerTranactionType = db.tblCustomerTranactionTypes.Find(id);
            if (tblCustomerTranactionType == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomerTranactionType);
        }

        // POST: CustomerTranactionTypes/Delete/6
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblCustomerTranactionType tblCustomerTranactionType = db.tblCustomerTranactionTypes.Find(id);
            db.tblCustomerTranactionTypes.Remove(tblCustomerTranactionType);
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
