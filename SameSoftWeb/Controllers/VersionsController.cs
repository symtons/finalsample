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
    public class VersionsController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: Versions
        public ActionResult Index()
        {
            return View(db.tblAppVersions.ToList());
        }



        public ActionResult App(string name)
        {

            var x = db.tblAppVersions.Where(a => a.App_Name == name).FirstOrDefault();
            if(x!=null)
            {
                return Content(x.Latest_Version.ToString());
            } else
            {
                return Content("0");
            }
            
        }


        // GET: Versions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAppVersion tblAppVersion = db.tblAppVersions.Find(id);
            if (tblAppVersion == null)
            {
                return HttpNotFound();
            }
            return View(tblAppVersion);
        }

        // GET: Versions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Versions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "App_ID,App_Name,Latest_Version,Last_Update_Date,Notes")] tblAppVersion tblAppVersion)
        {
            if (ModelState.IsValid)
            {
                db.tblAppVersions.Add(tblAppVersion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblAppVersion);
        }

        // GET: Versions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAppVersion tblAppVersion = db.tblAppVersions.Find(id);
            if (tblAppVersion == null)
            {
                return HttpNotFound();
            }
            return View(tblAppVersion);
        }

        // POST: Versions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "App_ID,App_Name,Latest_Version,Last_Update_Date,Notes")] tblAppVersion tblAppVersion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblAppVersion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblAppVersion);
        }

        // GET: Versions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAppVersion tblAppVersion = db.tblAppVersions.Find(id);
            if (tblAppVersion == null)
            {
                return HttpNotFound();
            }
            return View(tblAppVersion);
        }

        // POST: Versions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblAppVersion tblAppVersion = db.tblAppVersions.Find(id);
            db.tblAppVersions.Remove(tblAppVersion);
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
