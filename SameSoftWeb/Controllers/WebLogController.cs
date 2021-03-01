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
    public class WebLogController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: WebLog
        public ActionResult Index()
        {
            return View(db.tblWebLogs.OrderByDescending(a=>a.ID).ToList());
        }


        public ActionResult add(string type,string details)
        {
            db.tblWebLogs.Add(new tblWebLog

            {
                Log_Date = DateTime.Now,
                Type = type,
                Details = details
            });
            db.SaveChanges();
            return Content("added");
        }
        // GET: WebLog/Details/6
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblWebLog tblWebLog = db.tblWebLogs.Find(id);
            if (tblWebLog == null)
            {
                return HttpNotFound();
            }
            return View(tblWebLog);
        }

        // GET: WebLog/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WebLog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Log_Date,Type,Details")] tblWebLog tblWebLog)
        {
            if (ModelState.IsValid)
            {
                db.tblWebLogs.Add(tblWebLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblWebLog);
        }

        // GET: WebLog/Edit/6
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblWebLog tblWebLog = db.tblWebLogs.Find(id);
            if (tblWebLog == null)
            {
                return HttpNotFound();
            }
            return View(tblWebLog);
        }

        // POST: WebLog/Edit/6
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Log_Date,Type,Details")] tblWebLog tblWebLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblWebLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblWebLog);
        }

        // GET: WebLog/Delete/6
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblWebLog tblWebLog = db.tblWebLogs.Find(id);
            if (tblWebLog == null)
            {
                return HttpNotFound();
            }
            return View(tblWebLog);
        }

        // POST: WebLog/Delete/6
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblWebLog tblWebLog = db.tblWebLogs.Find(id);
            db.tblWebLogs.Remove(tblWebLog);
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
