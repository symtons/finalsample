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
    public class FeatureRequestsController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: FeatureRequests
        public ActionResult Index()
        {
            return View(db.tblFeatureRequests.ToList());
        }

        // GET: FeatureRequests/Details/6
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFeatureRequest tblFeatureRequest = db.tblFeatureRequests.Find(id);
            if (tblFeatureRequest == null)
            {
                return HttpNotFound();
            }
            return View(tblFeatureRequest);
        }

        // GET: FeatureRequests/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FeatureRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Feature_ID,Application_ID,User_ID,Customer_ID,Title,Description,Deadline,Importance,Requested_date,Status")] tblFeatureRequest tblFeatureRequest)
        {
            if (ModelState.IsValid)
            {
                db.tblFeatureRequests.Add(tblFeatureRequest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblFeatureRequest);
        }

        // GET: FeatureRequests/Edit/6
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFeatureRequest tblFeatureRequest = db.tblFeatureRequests.Find(id);
            if (tblFeatureRequest == null)
            {
                return HttpNotFound();
            }
            return View(tblFeatureRequest);
        }

        // POST: FeatureRequests/Edit/6
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Feature_ID,Application_ID,User_ID,Customer_ID,Title,Description,Deadline,Importance,Requested_date,Status")] tblFeatureRequest tblFeatureRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblFeatureRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblFeatureRequest);
        }

        // GET: FeatureRequests/Delete/6
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFeatureRequest tblFeatureRequest = db.tblFeatureRequests.Find(id);
            if (tblFeatureRequest == null)
            {
                return HttpNotFound();
            }
            return View(tblFeatureRequest);
        }

        // POST: FeatureRequests/Delete/6
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblFeatureRequest tblFeatureRequest = db.tblFeatureRequests.Find(id);
            db.tblFeatureRequests.Remove(tblFeatureRequest);
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
