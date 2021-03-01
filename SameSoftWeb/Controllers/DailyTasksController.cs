using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SameSoftWeb.Models;
using System.Data.Entity.Validation;

namespace SameSoftWeb.Controllers
{
    public class DailyTasksController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: DailyTasks
        public ActionResult Index()
        {

            int UserID = (int)Session["UserID"];

            var tblDialyTasks = db.tblDialyTasks.Where(a=>a.UserID== UserID || UserID==6).Include(t => t.tblCustomer).OrderByDescending(A=>A.Daily_Task_ID);
            return View(tblDialyTasks.ToList());
        }

        // GET: DailyTasks/Details/6
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDialyTask tblDialyTask = db.tblDialyTasks.Find(id);
            if (tblDialyTask == null)
            {
                return HttpNotFound();
            }
            return View(tblDialyTask);
        }

        // GET: DailyTasks/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.tblCustomers, "CustomerID", "Customer_Name");
            return View();
        }

        // POST: DailyTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(float Transportation, string Transportation_Mode, string Task_Decription,int CustomerID, DateTime Task_Date, [Bind(Include = "Daily_Task_ID,Transportation,Transportation_Mode,Task_Decription,CustomerID,Task_Date,From_Time,To_Time,Record_Date_Time,Last_Updated,Status")] tblDialyTask tblDialyTask)
        {
            int UserID = (int)Session["UserID"];
            tblDialyTask.Record_Date_Time = DateTime.UtcNow.AddHours(3);
            tblDialyTask.Status = "Check-In";
            tblDialyTask.UserID = UserID;
            string strTran_Nbr = DateTime.UtcNow.AddHours(3).ToString().GetHashCode().ToString("x");

            db.tblExpenses.Add(new tblExpense { Expense_Type_ID = 1, Cr = Transportation, Dr=0,Balance=0, Expense_Date = Task_Date, UserID = UserID,Payment_Method= "Cash", Period = Task_Date.Month, Year = Task_Date.Year, Note = "Cust "+ db.tblCustomers.Where(a=>a.CustomerID==CustomerID).Select(a=>a.Customer_Name).FirstOrDefault() + " ( " + Task_Decription + " ) " + Transportation_Mode, Status = "Posted", Tran_Nbr = strTran_Nbr });
            db.SaveChanges();

            if (ModelState.IsValid)
            {
                db.tblDialyTasks.Add(tblDialyTask);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.tblCustomers, "CustomerID", "Customer_Name", tblDialyTask.CustomerID);
            return View(tblDialyTask);
        }

        // GET: DailyTasks/Edit/6
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDialyTask tblDialyTask = db.tblDialyTasks.Find(id);
            if (tblDialyTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.tblCustomers, "CustomerID", "Customer_Name", tblDialyTask.CustomerID);
            return View(tblDialyTask);
        }

        // POST: DailyTasks/Edit/6
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
     //   public ActionResult Edit([Bind(Include = "Daily_Task_ID,Task_Decription,CustomerID,Task_Date,From_Time,To_Time,Record_Date_Time,Last_Updated,Status")] tblDialyTask tblDialyTask)

        public ActionResult Edit( int Daily_Task_ID, float Transportation, string Transportation_Mode, string Task_Decription,  Nullable<System.TimeSpan> To_Time)
        {
            var task = db.tblDialyTasks.Where(a => a.Daily_Task_ID == Daily_Task_ID).FirstOrDefault();
            if(task!=null)
            {
                task.Transportation = Transportation;
                task.Transportation_Mode = Transportation_Mode;
                task.To_Time = To_Time;
                task.Task_Decription = Task_Decription;
                task.Last_Updated = DateTime.UtcNow.AddHours(3);
                task.Status = "Check-Out";
            }
         
        


                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    string err = "";
                    foreach (var eve in e.EntityValidationErrors)
                    {
                     

                        err=String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            err= err+string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }


                    return Content(err);
                }
              
                return RedirectToAction("Index");
          
            
        }

        // GET: DailyTasks/Delete/6
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDialyTask tblDialyTask = db.tblDialyTasks.Find(id);
            if (tblDialyTask == null)
            {
                return HttpNotFound();
            }
            return View(tblDialyTask);
        }

        // POST: DailyTasks/Delete/6
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblDialyTask tblDialyTask = db.tblDialyTasks.Find(id);
            db.tblDialyTasks.Remove(tblDialyTask);
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
