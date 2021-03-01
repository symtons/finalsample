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
    public class ConnectionsController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: Connections
        public ActionResult Index()
        {
            var tblConnection_Data = db.tblConnection_Data.Include(t => t.tblUser);
            return View(tblConnection_Data.ToList());
        }

        // GET: Connections/Details/6
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblConnection_Data tblConnection_Data = db.tblConnection_Data.Find(id);
            if (tblConnection_Data == null)
            {
                return HttpNotFound();
            }
            return View(tblConnection_Data);
        }

        // GET: Connections/Create
        public ActionResult Create()
        {
            ViewBag.Generated_by = new SelectList(db.tblUsers, "UserID", "FullName");
            return View();
        }

        // POST: Connections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.

            public ActionResult Update_Expire(int id,int days)
        {
            DateTime d = DateTime.UtcNow.AddHours(3);

            var con = db.tblConnection_Data.Where(a => a.Connection_ID == id).FirstOrDefault();

            con.Expire_Date = d.AddDays(days);
            db.SaveChanges();
            return RedirectToAction("Index","Connections",new {msg="Connection Expire Date Updated" });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Connection_ID,Customer,Generated_by,Generated_Date,Pass_Code,Database_Server,Database_User,Database_Password,Database_Name")] tblConnection_Data tblConnection_Data,int expire)
        {


            DateTime d = DateTime.UtcNow.AddHours(3);
            tblConnection_Data.Expire_Date = d.AddDays(expire);

            tblConnection_Data.Generated_by = (int)Session["UserID"];
            tblConnection_Data.Generated_Date = DateTime.UtcNow.AddHours(3);

            if (ModelState.IsValid)
            {
                db.tblConnection_Data.Add(tblConnection_Data);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Generated_by = new SelectList(db.tblUsers, "UserID", "FullName", tblConnection_Data.Generated_by);
            return View(tblConnection_Data);
        }

        // GET: Connections/Edit/6
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblConnection_Data tblConnection_Data = db.tblConnection_Data.Find(id);
            if (tblConnection_Data == null)
            {
                return HttpNotFound();
            }
            ViewBag.Generated_by = new SelectList(db.tblUsers, "UserID", "FullName", tblConnection_Data.Generated_by);
            return View(tblConnection_Data);
        }

        // POST: Connections/Edit/6
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Connection_ID,Customer,Generated_by,Generated_Date,Pass_Code,Database_Server,Database_User,Database_Password,Database_Name")] tblConnection_Data tblConnection_Data)
        {
            tblConnection_Data.Generated_by = (int)Session["UserID"];
            tblConnection_Data.Generated_Date = DateTime.UtcNow.AddHours(3);

            if (ModelState.IsValid)
            {
                db.Entry(tblConnection_Data).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Generated_by = new SelectList(db.tblUsers, "UserID", "FullName", tblConnection_Data.Generated_by);
            return View(tblConnection_Data);
        }

        // GET: Connections/Delete/6
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblConnection_Data tblConnection_Data = db.tblConnection_Data.Find(id);
            if (tblConnection_Data == null)
            {
                return HttpNotFound();
            }
            return View(tblConnection_Data);
        }

        // POST: Connections/Delete/6
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblConnection_Data tblConnection_Data = db.tblConnection_Data.Find(id);
            db.tblConnection_Data.Remove(tblConnection_Data);
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
