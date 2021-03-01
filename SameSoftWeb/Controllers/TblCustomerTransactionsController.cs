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
    public class TblCustomerTransactionsController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: TblCustomerTransactions
        public ActionResult Index()
        {
            return View(db.TblCustomerTransactions.ToList());
        }

        // GET: TblCustomerTransactions/Details/6
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblCustomerTransaction tblCustomerTransaction = db.TblCustomerTransactions.Find(id);
            if (tblCustomerTransaction == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomerTransaction);
        }

        // GET: TblCustomerTransactions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TblCustomerTransactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Transction_ID,Transaction_Date,Transaction_type_id,Debit,Credit,Customer_ID,UserID,Description,Due_Date,Status,Note,User_ID")] TblCustomerTransaction tblCustomerTransaction)
        {
            if (ModelState.IsValid)
            {
                db.TblCustomerTransactions.Add(tblCustomerTransaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblCustomerTransaction);
        }

        // GET: TblCustomerTransactions/Edit/6
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblCustomerTransaction tblCustomerTransaction = db.TblCustomerTransactions.Find(id);
            if (tblCustomerTransaction == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomerTransaction);
        }

        // POST: TblCustomerTransactions/Edit/6
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Transction_ID,Transaction_Date,Transaction_type_id,Debit,Credit,Customer_ID,UserID,Description,Due_Date,Status,Note,User_ID")] TblCustomerTransaction tblCustomerTransaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblCustomerTransaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblCustomerTransaction);
        }

        // GET: TblCustomerTransactions/Delete/6
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblCustomerTransaction tblCustomerTransaction = db.TblCustomerTransactions.Find(id);
            if (tblCustomerTransaction == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomerTransaction);
        }

        // POST: TblCustomerTransactions/Delete/6
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TblCustomerTransaction tblCustomerTransaction = db.TblCustomerTransactions.Find(id);
            db.TblCustomerTransactions.Remove(tblCustomerTransaction);
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
