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
    public class ApplicationsController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: Applications
        public ActionResult Index()
        {
            return View(db.tblApplications.ToList());
        }

        // GET: Applications/Details/6
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblApplication tblApplication = db.tblApplications.Find(id);
            if (tblApplication == null)
            {
                return HttpNotFound();
            }
            return View(tblApplication);
        }

        // GET: Applications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Applications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Application_ID,Application_Name,Type,Description")] tblApplication tblApplication)
        {
            if (ModelState.IsValid)
            {
                db.tblApplications.Add(tblApplication);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblApplication);
        }

        // GET: Applications/Edit/6
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblApplication tblApplication = db.tblApplications.Find(id);
            if (tblApplication == null)
            {
                return HttpNotFound();
            }
            return View(tblApplication);
        }

        // POST: Applications/Edit/6
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Application_ID,Application_Name,Type,Description")] tblApplication tblApplication)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblApplication).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblApplication);
        }

        // GET: Applications/Delete/6
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblApplication tblApplication = db.tblApplications.Find(id);
            if (tblApplication == null)
            {
                return HttpNotFound();
            }
            return View(tblApplication);
        }

        // POST: Applications/Delete/6
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblApplication tblApplication = db.tblApplications.Find(id);
            db.tblApplications.Remove(tblApplication);
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
