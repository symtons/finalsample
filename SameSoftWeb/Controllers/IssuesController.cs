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
    public class IssuesController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: Problems
        public ActionResult Index(int? appid,string type)
        {
            int id = 0;
            if(appid!=null)
            {
                if(appid!=0)
                {
                    id = (int)appid;
                    ViewBag.appid = appid;
                    ViewBag.type = type;
                }
            }
            return View(db.tblProblems.Where(a=>appid>0 && (a.Application_ID==id || id==0) && (a.Type==type || type.Length<3 ||type=="")).OrderBy(a=>a.ErrorDate).ThenBy(a=>a.Type).ToList());
        }

        // GET: Problems/Details/6
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblProblem tblProblem = db.tblProblems.Find(id);
            if (tblProblem == null)
            {
                return HttpNotFound();
            }
            return View(tblProblem);
        }

        // GET: Problems/Create
       
        public ActionResult ReportIssue(int? Appid,string User,int? Customer,int? UserID)
        {
            if(Appid!=null)
            {
                Session["customer_id"] = Customer;
                Session["FullName"] = User;
                Session["AppID"] = Appid;
                Session["UserID"] = UserID;
                Session["Role"] = "Customer";
                Session["Customer_Name"] = db.tblCustomers.Where(a => a.CustomerID == Customer).Select(a => a.Customer_Name).FirstOrDefault();

            }

            return View();
        }

        // POST: Problems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Priority,Problem_ID,Customer_ID,Title,Description,UserID,Status,Solution_Note,ErrorDate,Application_ID,User,Type")] tblProblem tblProblem)
        {
            if (ModelState.IsValid)
            {

                tblProblem.Status = "Pending";
                tblProblem.ErrorDate = DateTime.UtcNow.AddHours(3);


                db.tblProblems.Add(tblProblem);
                db.SaveChanges();


                if(Session["Role"].ToString()=="Customer")
                {
                    return RedirectToAction("Index","Customer",new {r=1 });
                }
                return RedirectToAction("Index");




            }

            return View(tblProblem);
        }


        public ActionResult UpdateStatus(string status,int id,string note)
        {

            SameSoftWeb.Models.SameSoftwareWebEntities db = new SameSoftWeb.Models.SameSoftwareWebEntities();

            var p = db.tblProblems.Where(a => a.Problem_ID == id).FirstOrDefault();
            if(p!=null)
            {
                p.Status = status;
                p.Solution_Note=note;
                db.SaveChanges();
            }
            return RedirectToAction("Details", "Issues", new { id = id });

        }


        // GET: Problems/Edit/6
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblProblem tblProblem = db.tblProblems.Find(id);
            if (tblProblem == null)
            {
                return HttpNotFound();
            }
            return View(tblProblem);
        }

        // POST: Problems/Edit/6
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Priority,Problem_ID,Customer_ID,Title,Description,UserID,Status,Solution_Note,ErrorDate,Application_ID,User,Type")] tblProblem tblProblem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblProblem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblProblem);
        }

        // GET: Problems/Delete/6
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblProblem tblProblem = db.tblProblems.Find(id);
            if (tblProblem == null)
            {
                return HttpNotFound();
            }
            return View(tblProblem);
        }

        // POST: Problems/Delete/6
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblProblem tblProblem = db.tblProblems.Find(id);
            db.tblProblems.Remove(tblProblem);
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

        public ActionResult Done(int id,int appid)
        {
            var prob = db.tblProblems.Where(A => A.Problem_ID == id).FirstOrDefault();
            if(prob!=null)
            {
                prob.Status = "Solved";
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Issues", new { appid = appid });

        }

        public ActionResult UnDone(int id, int appid)
        {
            var prob = db.tblProblems.Where(A => A.Problem_ID == id).FirstOrDefault();
            if (prob != null)
            {
                prob.Status = "Pending";
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Issues", new { appid = appid });

        }

    }
}
