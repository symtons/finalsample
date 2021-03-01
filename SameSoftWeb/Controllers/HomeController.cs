using SameSoftWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SameSoftWeb.Controllers
{

    public class HomeController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();
        public ActionResult Index(int? id)
        {
           ViewBag.id = id;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult RegistredProblem()
        {
            ViewBag.msg = "Thank You";
            return View("Index");
        }
        public ActionResult Products()
        {
       return View();
        }
        public ActionResult Customers()
        {
            return View();
        }

        public ActionResult ReceiptVoucher(int id)
        {

            ViewBag.id = id;
            return View();

        }

        public ActionResult CashfLow(DateTime? from,DateTime? to,string type)
        {

            ViewBag.type = type;
            ViewBag.from     = from;
            ViewBag.to = to;
            return View();
        }

        public ActionResult Expense()
        {
          
            return View();
        }

        [HttpPost]
        public ActionResult Expense(DateTime trandate, float amount, string desc)
        {

            int userid = (int)Session["UserID"];
            string inwords = helper.mnc.ToWords(amount.ToString());
           


            db.tblCashFlows.Add(new tblCashFlow
            {
                Credit = amount,
                Debit = 0,
                Description = desc,
                Tran_Date = trandate,
                Type = "Expense",
                UserID = userid
            });
            db.SaveChanges();

          
            db.tblPayment_Voucher.Add(new tblPayment_Voucher
            {
                Amount = amount,
             
                Payment_Date = trandate,
                UserID = userid,
                In_Word = inwords,
                Description = desc,
                Payee="Expense",
                Company_ID=1

            });
            db.SaveChanges();
            //var MaxReceipt = db.tblReceipt_Voucher.Select(a => a.Voucher_ID).Max();


            //return RedirectToAction("ReceiptVoucher", "Home", new { id = MaxReceipt });
            ViewBag.msg = "Expense Recoded Successfully";
            return View();
        }
    }
}