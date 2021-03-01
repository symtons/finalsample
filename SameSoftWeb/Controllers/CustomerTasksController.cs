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
    public class CustomerTasksController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: CustomerTasks
        public ActionResult Index()
        {
            return View(db.CustomerTasks.ToList());
        }

        // GET: CustomerTasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerTask customerTask = db.CustomerTasks.Find(id);
            if (customerTask == null)
            {
                return HttpNotFound();
            }
            return View(customerTask);
        }

        // GET: CustomerTasks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Customer_id,Name,Note")] CustomerTask customerTask)
        {
            if (ModelState.IsValid)
            {
                db.CustomerTasks.Add(customerTask);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customerTask);
        }

        // GET: CustomerTasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerTask customerTask = db.CustomerTasks.Find(id);
            if (customerTask == null)
            {
                return HttpNotFound();
            }
            return View(customerTask);
        }

        // POST: CustomerTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Customer_id,Name,Note")] CustomerTask customerTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customerTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customerTask);
        }

        // GET: CustomerTasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerTask customerTask = db.CustomerTasks.Find(id);
            if (customerTask == null)
            {
                return HttpNotFound();
            }
            return View(customerTask);
        }

        // POST: CustomerTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CustomerTask customerTask = db.CustomerTasks.Find(id);
            db.CustomerTasks.Remove(customerTask);
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
