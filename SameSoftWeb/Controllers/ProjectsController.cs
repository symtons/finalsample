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
    public class ProjectsController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: Projects
        public ActionResult Index()
        {
            return View(db.tblProjects.ToList().OrderBy(a=>a.Status));
        }

        // GET: Projects/Details/6
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblProject tblProject = db.tblProjects.Find(id);
            if (tblProject == null)
            {
                return HttpNotFound();
            }
            return View(tblProject);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Project_ID,Customer_Name,Creation_Date,Application,Total_Cost,Status,Quotation_Link,PaymentAgreement_Link,Priority,Created_by,Payments,Agreement_Status,Project_Title")] tblProject tblProject)
        {
            if (ModelState.IsValid)
            {
                db.tblProjects.Add(tblProject);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblProject);
        }

        // GET: Projects/Edit/6
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblProject tblProject = db.tblProjects.Find(id);
            if (tblProject == null)
            {
                return HttpNotFound();
            }
            return View(tblProject);
        }

        // POST: Projects/Edit/6
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Project_ID,Customer_Name,Creation_Date,Application,Total_Cost,Status,Quotation_Link,PaymentAgreement_Link,Priority,Created_by,Payments,Agreement_Status,Project_Title")] tblProject tblProject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblProject).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblProject);
        }

        // GET: Projects/Delete/6
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblProject tblProject = db.tblProjects.Find(id);
            if (tblProject == null)
            {
                return HttpNotFound();
            }
            return View(tblProject);
        }

        // POST: Projects/Delete/6
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblProject tblProject = db.tblProjects.Find(id);
            db.tblProjects.Remove(tblProject);
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
