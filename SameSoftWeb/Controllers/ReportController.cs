using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SameSoftWeb.Controllers
{



    public class ReportController : Controller
    {
        // GET: Report
        SameSoftWeb.Models.SameSoftwareWebEntities db = new SameSoftWeb.Models.SameSoftwareWebEntities();

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult CashFlowStatement(DateTime? from, DateTime? to, int? acc, string status)
        {
        //    UsersController u = new UsersController();
        //    u.checkaccess(49);

            ViewBag.from = from;
            ViewBag.to = to;
            ViewBag.acc = acc;
            ViewBag.status = status;
         

            ViewBag.accname = db.tblChart_Of_Accounts.Where(a => a.Account_Number == acc).Select(a => a.Account_Name).FirstOrDefault();

            return View();
        }





        public ActionResult ReceiptVouchers(DateTime? from, DateTime? to, int? user)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(47);

            ViewBag.from = from;
            ViewBag.to = to;
            ViewBag.user = user;
         
            return View();
        }

        public ActionResult PaymentVouchers(DateTime? from, DateTime? to, int? user)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(48);

            ViewBag.from = from;
            ViewBag.to = to;
            ViewBag.user = user;

            return View();
        }




        public ActionResult Expenses(DateTime? from, DateTime? to,int? type,int? id)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(45);

            ViewBag.from = from;
            ViewBag.to = to;
      
            ViewBag.type = type;
            ViewBag.id = id;
            return View();
        }

        
       
  
   
        //public ActionResult BalanceSheet(DateTime? to)
        //{
        //    UsersController u = new UsersController();
        //    u.checkaccess(51);

        //    ViewBag.to = to;
        //    return View();


        //}

        //public ActionResult IncomeStatement(DateTime? from, DateTime? to)
        //{
        //    UsersController u = new UsersController();
        //    u.checkaccess(50);

        //    if (from != null)
        //    {
        //        ViewBag.from = from;
        //        ViewBag.to = to;
        //    }
        //    return View();

        //}

        //public ActionResult TrialBalance(DateTime? from, DateTime? to)
        //{
        //    UsersController u = new UsersController();
        //    u.checkaccess(52);

        //    ViewBag.from = from;
        //    ViewBag.to = to;
        //    return View();
        //}

        public ActionResult TransactionLog(DateTime? from, DateTime? to,string type,int? user)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(68);

            ViewBag.from = from;
            ViewBag.to = to;
            ViewBag.type = type;
            ViewBag.user = user;

            return View();
        }



        

        //      public ActionResult OwnerTransactions(DateTime? from, DateTime? to, int id)
        //{
        //    ViewBag.from = from;
        //    ViewBag.to = to;
        //    ViewBag.id = id;
        

        //    return View();
        //}













        public ActionResult PurchaseOrders()
        {
            return View();
        }


        public ActionResult AvailableInventory(int? divid, DateTime? to)
        {


           
                divid = 1;
         


            ViewBag.divid = divid;
            ViewBag.to = to;

            return View();
        }
        [HttpPost]
        public ActionResult GetAvailableInventory(int divid, DateTime date)
        {
            decimal? i = 0;

            int role_id = (int)Session["RoleID"];


            var data = from s in db.sp_get_qty_available_by_division_date(divid + 62176, date).ToList()
                       select new
                       {
                           item_id = s.Item_Id,
                           Item_name = s.Item_Name,
                           category = s.Category,
                           qtyin = s.QTYIN,
                           qtyout = s.QTY_Out,
                           qtyavailable = s.Available_Qty,
                           cost = s.Unit_Cost
                       };

            // Cashier and Admin can see costs
            if (role_id == 1 || role_id == 2)
            {
                data = from s in db.sp_get_qty_available_by_division_date(divid, date).ToList()
                       select new
                       {
                           item_id = s.Item_Id,
                           Item_name = s.Item_Name,
                           category = s.Category,
                           qtyin = s.QTYIN,
                           qtyout = s.QTY_Out,
                           qtyavailable = s.Available_Qty,
                           cost = s.Unit_Cost
                       };

            }
            else
            {
                data = from s in db.sp_get_qty_available_by_division_date(divid, date).ToList()
                       select new
                       {
                           item_id = s.Item_Id,
                           Item_name = s.Item_Name,
                           category = s.Category,
                           qtyin = s.QTYIN,
                           qtyout = s.QTY_Out,
                           qtyavailable = s.Available_Qty,
                           cost = i
                       };
            }


            return Json(data, JsonRequestBehavior.AllowGet);

        }
        



        public ActionResult ReceivedInventory(int? divid, DateTime? from, DateTime? to, string lot)
        {
            ViewBag.lot = lot;

            if (divid != null)
            {
                ViewBag.divid = divid;
            }
            else
            {
                divid = 1;
            }

            ViewBag.from = from;
            ViewBag.to = to;

            return View();

        }


        public ActionResult InventorySummary()
        {
            return View();
        }


        public ActionResult Sales_Summary(int? divid, DateTime? from, DateTime? to)
        {
            if (divid != null)
            {
                ViewBag.divid = divid;
            }
            else
            {
                divid = 1;
            }

            ViewBag.from = from;
            ViewBag.to = to;
            return View();
        }


    }
}