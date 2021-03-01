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
    public class ClientsController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: Customers
        public ActionResult Index(string Type, string Name, string Status)
        {
            //ViewBag.cnt = db.tblCustomers.Select(a => a.CustomerID).Count();
            ViewBag.type = Type;
            ViewBag.name = Name;
            ViewBag.status = Status;

            return View("Index");
        }

        // GET: Customers/Details/6
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomer tblCustomer = db.tblCustomers.Find(id);
            if (tblCustomer == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Maintainance,Contract_Date,CustomerID,Customer_Name,Address,Cutomer_Type,Status,Contact_Numbers")] tblCustomer tblCustomer)
        {
            if (ModelState.IsValid)
            {
                db.tblCustomers.Add(tblCustomer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblCustomer);
        }

        // GET: Customers/Edit/6
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomer tblCustomer = db.tblCustomers.Find(id);
            if (tblCustomer == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomer);
        }

        // POST: Customers/Edit/6
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Maintainance,Contract_Date,Contact_Numbers,CustomerID,Customer_Name,Address,Cutomer_Type,Status")] tblCustomer tblCustomer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblCustomer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblCustomer);
        }

        // GET: Customers/Delete/6
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomer tblCustomer = db.tblCustomers.Find(id);
            if (tblCustomer == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomer);
        }

        // POST: Customers/Delete/6
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblCustomer tblCustomer = db.tblCustomers.Find(id);
            db.tblCustomers.Remove(tblCustomer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult PostTrans(int customer_id, int tran_id, float amount, DateTime duedate, DateTime trandate, string desc)
        {


            int user_id = (int)Session["UserID"];
            db.TblCustomerTransactions.Add(new TblCustomerTransaction
            {
                Credit = 0,
                Debit = amount,
                Description = desc,
                Due_Date = duedate,
                Status = "Open",
                Customer_ID = customer_id,
                Transaction_type_id = tran_id,
                UserID = user_id,
                Transaction_Date = trandate

            });
            db.SaveChanges();

            return Content("<br><br><center><h1> Charges Posted Succcessfully");
        }
        public ActionResult PostCharges()
        {
            return View();
        }
        public ActionResult MultiPostCharges()
        {
            return View();
        }

        public ActionResult getTrans(int id)
        {
            var trans = db.TblCustomerTransactions.Select(c => new
            {
                c.Status,
                c.Customer_ID,
                c.Debit,
                ID = c.Transction_ID,
                Text = c.Description + "($" + c.Debit + ")"
            }).Where(a => a.Customer_ID == id && a.Debit > 0
            && a.Status != "Paid"
            );

            return Json(trans, JsonRequestBehavior.AllowGet);
        }




        public ActionResult DeleteTrans(int id)
        {

            if (Session["Role"].ToString() == "Admin")
            {
                var tran = db.TblCustomerTransactions.Where(a => a.Transction_ID == id).FirstOrDefault();
                if (tran != null)
                {
                    db.TblCustomerTransactions.Remove(tran);
                    db.SaveChanges();
                }

                return Content("<h3> Transction Deleted Successfully");
            }

            return View();
        }
        public ActionResult FeeCollectionReport(int? all, DateTime? vdate)
        {
            ViewBag.vdate = vdate;
            ViewBag.all = all;

            return View();
        }


        public ActionResult PostMultiTrans(int[] id, float fee, string desc, int trantype, DateTime duedate, DateTime trandate)
        {


            int user_id = (int)Session["UserID"];
            int c_id = 0;
            int cnt = 0;
            for (int x = 0; x <= id.Length - 1; x++)
            {

                c_id = id[x];
                cnt += 1;

                db.TblCustomerTransactions.Add(new TblCustomerTransaction
                {
                    Credit = 0,
                    Debit = fee,
                    Description = desc,
                    Due_Date = duedate,
                    Status = "Open",
                    Customer_ID = c_id,
                    Transaction_type_id = trantype,
                    UserID = user_id
                    ,
                    Transaction_Date = trandate

                });
                db.SaveChanges();



            }

            return Content("<br><br><center><h1> Charges Posted Succcessfully For " + cnt.ToString());

        }



        public ActionResult statement(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult Balance()
        {

            return View();
        }



        public ActionResult Payment()
        {

            return View();
        }


        [HttpPost]

        public ActionResult Payment(DateTime trandate, float amount, int customer_id, string desc, int tran_id)
        {

            var tran_to_pay = db.TblCustomerTransactions.Where(a => a.Transction_ID == tran_id).FirstOrDefault();

            if (amount >= tran_to_pay.Debit)
            {
                tran_to_pay.Status = "Paid";
                db.SaveChanges();
            }
            else
            {
                tran_to_pay.Status = "Partially Paid";
                db.SaveChanges();
            }
            int userid = (int)Session["UserID"];
            string inwords = helper.mnc.ToWords(amount.ToString());
            db.TblCustomerTransactions.Add(new TblCustomerTransaction
            {
                Credit = amount,
                Debit = 0,
                Customer_ID = customer_id,
                Status = "Paid",
                UserID = userid,
                Description = desc,
                Transaction_type_id = 2,
                Transaction_Date = trandate

            });
            db.SaveChanges();


            db.tblCashFlows.Add(new tblCashFlow
            {
                Credit = 0,
                Debit = amount,
                Description = desc,
                Tran_Date = trandate,
                Type = "Cash In",
                UserID = userid
            });
            db.SaveChanges();

            float? balance = (float)db.sp_get_Customer_Balanace().Where(a => a.CustomerID == customer_id).Select(a => a.Balance).FirstOrDefault();
            var custname = db.tblCustomers.Where(a => a.CustomerID == customer_id).Select(a => a.Customer_Name).FirstOrDefault();



            db.tblReceipt_Voucher.Add(new tblReceipt_Voucher
            {
                Amount = amount,
                Balance = balance,
                Payee = custname,
                Receipt_Date = trandate,
                UserID = userid,
                In_Word = inwords,
                Description = desc

            });
            db.SaveChanges();
            var MaxReceipt = db.tblReceipt_Voucher.Select(a => a.Voucher_ID).Max();


            return RedirectToAction("ReceiptVoucher", "Home", new { id = MaxReceipt });

        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult AddBackupLog(string Customer, string log)
        {

            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/CustomerBackupLog.txt", Customer + " : " + DateTime.UtcNow.AddHours(3).ToString() + log);
                return Content("Log Saved");


        }

    }
}
