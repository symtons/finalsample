using SameSoftWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SameSoftWeb.helper;
namespace SameSoftWeb.Controllers
{


   
    public class SalesController : Controller
    {
        // GET: Sales
        SameSoftWeb.Models.SameSoftwareWebEntities db = new SameSoftWeb.Models.SameSoftwareWebEntities();
        public ActionResult Index()
        {
            return RedirectToAction("index","Home");
        }


        public ActionResult NewOrder()
        {
            //UsersController u = new UsersController();
            //u.checkaccess(33);
            return View();
        }

        [HttpGet]
        public ActionResult get_batches(int id)
        {



            double? x = db.sp_check_item_batch(id).FirstOrDefault();
            if (x == null || x==0)
            {
                var data = from s in db.sp_get_items_with_batch_qty(id)
                           select new
                           {
                               id = s.Item_Id,
                               name = s.Item,
                               qty = s.QTY,
                               expire = DateTime.Parse(s.Expire_Date.ToString()).ToShortDateString(),
                               batch=s.Batch,
                               price=s.Sales_price
                           };





                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(x, JsonRequestBehavior.AllowGet);
            }
           

        }



        public ActionResult add_sales_order(int[] item_id, float[] qty,string[] batch,float[] price,float discount,string payment_method,int? customer_id,DateTime? trandate ,float? cash_payment)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(33);

            if (cash_payment==null)
            {
                cash_payment = 0;
            }

            int userid = (int)Session["UserID"];
            int business_division = (int)1;


            DateTime vdate = helper.sitemethods.getdate();
            if (trandate != null)
            {
                DateTime dt = (DateTime)trandate;
                if (dt.Date == vdate.Date)
                {
                    vdate = helper.sitemethods.getdate();
                }
                else
                {
                    vdate = dt;
                }
            }



            double balance = 0;
            var Cust_balance = db.sp_Get_Customer_with_Balance2(customer_id).FirstOrDefault();
            if (Cust_balance != null)
            {
                balance = Cust_balance.Total;
            }


            // Validate
            db.Database.BeginTransaction();
  int inventory_type_id = (int)db.Inventory_Types.Where(a => a.Inventory_Type == "Sales").Select(a => a.Inventory_type_id).FirstOrDefault();
            db.tblInventory_Transactions.Add(new Models.tblInventory_Transactions { Business_Division_Id = business_division, Total = 0, Tran_Date = vdate, Status = "Open", UserID = userid, Payment_Method = payment_method, Inventory_type_id = inventory_type_id, Tran_Nbr = "TRAN123",Discount=discount,Customer_ID= customer_id ,Cash_Payment=cash_payment });
            db.SaveChanges();

            int last_inventory_id = 0;
            last_inventory_id = db.tblInventory_Transactions.Select(a => a.Inventory_Tran_ID).Max();

            for (int x = 0; x <= item_id.Length - 1; x++)
            {
                int itemid = item_id[x];
                var item_Detail = db.Items.Where(a => a.Item_Id == itemid).FirstOrDefault();
                db.tblInventory_Transaction_Details.Add(new Models.tblInventory_Transaction_Details { Inventory_Tran_ID = last_inventory_id, Item_Id = item_id[x], Qty_In = 0, Qty_Out = qty[x], Unit_Price = (float)item_Detail.Unit_Cost,Sales_price=price[x], Batch = batch[x] });
            }
            db.SaveChanges();
 float total = 0;
 total = (float)db.tblInventory_Transaction_Details.Where(a => a.Inventory_Tran_ID == last_inventory_id).Select(a => a.Qty_Out * a.Sales_price).Sum();
            var p = db.tblInventory_Transactions.Where(f => f.Inventory_Tran_ID == last_inventory_id).FirstOrDefault();
            p.Total = total;

            if (payment_method.Contains("Invoice"))
            {
            }else
            {
                p.Cash_Payment = total;
            }


                int serialno = 0;
            int? sr = db.tblInvoices.Where(a => a.Invoice_id == 1).Select(a => a.Invoice_No).FirstOrDefault();
            if (sr==null || sr==0)
            {
                serialno = int.Parse(db.tblSettings.Where(a => a.ID == 1).Select(a => a.Value).FirstOrDefault());
            }
            else
            {
                serialno = (int)sr + 1;
            }
            p.Invoice_No = serialno;

            db.SaveChanges();
            db.Database.CurrentTransaction.Commit();




            var InvoiceUpdate = db.tblInvoices.FirstOrDefault();
            if (InvoiceUpdate != null)
            {
                InvoiceUpdate.Invoice_No = serialno;
                db.SaveChanges();
            }



            //add log
            sitemethods.addlog("Add Data", "Add Sales Inv # (" + serialno + ")  Amonut of   " + total, business_division, userid);


            if (payment_method.Contains("Invoice"))
            {
                // Add Customer Transaction

                int Cust_Tran_Type = 0;
                    Cust_Tran_Type = db.tblCustomer_Tran_Type.Where(a => a.Tran_Type == "Purchase").Select(a => a.Cust_Tran_Type_ID).FirstOrDefault();
                    int? cust_tran_id = null;
                    try
                    {
                        cust_tran_id = db.tblCustomer_Transaction.Where(a => a.CustomerID == customer_id && a.Status == "Open").Select(a => a.Cust_Tran_ID).Max();
                    }
                    catch (Exception ex) { }


                    if (cust_tran_id == null)
                    {
                        db.tblCustomer_Transaction.Add(new tblCustomer_Transaction { Amount = total - discount, CustomerID = (int)customer_id, Status = "Open", Tran_Date = vdate, UserID = userid });
                        db.SaveChanges();
                        cust_tran_id = db.tblCustomer_Transaction.Where(a => a.UserID == userid).Select(a => a.Cust_Tran_ID).Max();
                    }
                    db.tblCustomer_Transaction_Detail.Add(new tblCustomer_Transaction_Detail { Debit = total - discount, Credit = 0, Cust_Tran_Type_ID = Cust_Tran_Type, Status = "Open", UserID = userid, Description = "Custmer Invoice Order #" + serialno, Inventory_Tran_ID = last_inventory_id, Tran_Nbr = "N/A", Trans_Date = vdate, Cust_Tran_ID = (int)cust_tran_id , Invoice_No=serialno,Balance=(float)balance});
                    db.SaveChanges();
                return RedirectToAction("PrintInvoice", "Accounting", new { id = serialno});
               
            }

            return RedirectToAction("PrintInvoice", "Accounting", new { id = serialno });
        }



        [HttpGet]
        public ActionResult Receipt(int? id)
    {
            if (id==null)
            {
                return RedirectToAction("Dashboard","Home");
 }

            ViewBag.id = id;
            return View();
    }

        [HttpGet]
        public ActionResult Quotation(int? id)
        {
            
            if (id == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.id = id;
            return View();
        }




        public ActionResult CancelSales()
        {
            //UsersController u = new UsersController();
            //u.checkaccess(36);
            return View();
        }




        public ActionResult GetItemIDbyBarcode(string barcode)
        {
            object divid = 1;
            int id = 0;
            if (divid != null)
            {
                id = (int)divid;
            }



            var item = db.tblInventory_Transaction_Details.Where(a => a.Barcode == barcode).Select(a => a.Item_Id).Max();

            if (item != null)
            {
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
       
        }



        public ActionResult CancelSale(int id)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(36);
            var inv = db.tblInventory_Transactions.Where(a => a.Inventory_Tran_ID == id).FirstOrDefault();

            if (inv != null)
            {
                inv.Status = "Cancelled";
                db.SaveChanges();
                int divid = (int)1;
                int userid = (int)Session["UserID"];
                //add log
                sitemethods.addlog("Cancel Data", "Cancel Sales Inv # (" + id + ")", divid, userid);

                return RedirectToAction("success", "message", new { msg = "Sales Order #"+id+"  Cancelled Successfully !" });

            }

            return RedirectToAction("success", "message", new { msg = "Purchase  Not Cancelled !" });


        }

        public ActionResult UpdateSales()
        {
            //UsersController u = new UsersController();
            //u.checkaccess(37);
            return View();
        }

        public ActionResult SalesReturn(int id)
        {

            ViewBag.id = id;
            return View();

        }

        public ActionResult DoSalesReturn(int[] tranid,int[] newqty,int[] item_id,int?[] qtyreturned)
        {


            int receipt_id = 0;
            for (int x=0;x<=tranid.Length-1;x++)
            {
                if (qtyreturned[x]!=null)
                {
                    var id = tranid[x];
                    var rw = db.tblInventory_Transaction_Details.Where(a => a.Inventory_Tran_Detail_ID == id).FirstOrDefault();
                    if(rw!=null)
                    {
                        receipt_id = rw.Inventory_Tran_ID;
                        rw.Qty_Out = newqty[x];
                        db.SaveChanges();
                    }

                    int divid = (int)1;
                    int userid = (int)Session["UserID"];
                    //add log
                    sitemethods.addlog("Update Data", "Quantity Update for Sales Inv # (" + id + ")", divid, userid);


                }
            }

                
            // if posted ; do all needed acc trans.
               return RedirectToAction("Receipt", "Sales", new { id = receipt_id });

        }

        public ActionResult ExpiredItems(DateTime? date,int? divid)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(26);
            if (date != null && divid != null)
            {
                ViewBag.date = date;
                ViewBag.divid = divid;
            }

            return View();

        }

        public ActionResult ReorderLevelItems(int? divid)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(27);
            if ( divid != null)
            {
           
                ViewBag.divid = divid;
            }

            return View();

        }

        public ActionResult Installment(int id,int customer)
        {

            ViewBag.id = id;
            ViewBag.customer = customer;

            return View();

        }


        public ActionResult addInstallmentPlan(int id, int customer, float[] persents, DateTime[] dates)
        {

            int userid = (int)Session["UserID"];
            DateTime vdate = DateTime.Now.Date;
            var ordertotal = db.tblInventory_Transactions.Where(a => a.Inventory_Tran_ID == id).Select(a => a.Total - a.Discount).FirstOrDefault();

            int divid = (int)1;




            // Delete Prev Installment Plans.
            string old = "";
            IEnumerable<InstallmentPlan> i = db.InstallmentPlans.Where(a => a.OrderId == id).ToList();
            if (i.Count() > 0)
            {
                old = "; Previous Installment Plan was Removed.";
                db.InstallmentPlans.RemoveRange(i);
                db.SaveChanges();
            }


            int? trans_detail_id = (int)db.tblCustomer_Transaction_Detail.Where(a => a.Inventory_Tran_ID == id).Select(a => a.Cust_Transaction_Detail_id).FirstOrDefault();

            if (trans_detail_id == null)
            {
                return RedirectToAction("error", "message", new { msg = "Could not Create Installment Plan from Transaction # " + id + " ; No Customer Invoice Associated with this Transaction" });
            }


            //add log
            sitemethods.addlog("Add Data", "Add Installment Plan for Sales Inv # (" + id + ") " + old, divid, userid);

            float total = 0;
            for (int x = 0; x <= persents.Length - 1; x++)
            {
                total = ordertotal * (persents[x] / 100);
                total = (float)Math.Round(total, 2);

                db.InstallmentPlans.Add(new Models.InstallmentPlan { OrderId = id, Customer_Id = customer, paid = 0, status = "Open", payment_persent = persents[x], payment_date = dates[x], Payment_total = total, Trans_Detail_ID = trans_detail_id });
                db.SaveChanges();
            }
            return RedirectToAction("Receipt", "Sales", new { id = id });
        }



        public ActionResult SalesQuotation()
        {
            //UsersController u = new UsersController();
            //u.checkaccess(34);
            return View();
        }
        public ActionResult add_quotation(int[] item_id, float[] qty, string[] batch, float[] price, float discount, string payment_method, int? customer_id,DateTime? trandate)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(34);

            int userid = (int)Session["UserID"];
            int business_division = (int)1;


            DateTime vdate = helper.sitemethods.getdate();
            if (trandate != null)
            {
                DateTime dt = (DateTime)trandate;
                if (dt.Date == vdate.Date)
                {
                    vdate = helper.sitemethods.getdate();
                }
                else
                {
                    vdate = dt;
                }
            }

            // Validate
            db.Database.BeginTransaction();
           // int inventory_type_id = (int)db.Inventory_Types.Where(a => a.Inventory_Type == "Sales").Select(a => a.Inventory_type_id).FirstOrDefault();
            db.tblQuotations.Add(new tblQuotation { Business_Division_Id = business_division, Total = 0, Tran_Date = vdate, Status = "Open", UserID = userid, Discount = discount,Tran_Nbr="--", Customer_ID = customer_id });
            db.SaveChanges();
 int last_inventory_id = 0;
            last_inventory_id = db.tblQuotations.Select(a => a.Quotation_ID).Max();
  for (int x = 0; x <= item_id.Length - 1; x++)
            {
                int itemid = item_id[x];
                var item_Detail = db.Items.Where(a => a.Item_Id == itemid).FirstOrDefault();
                db.Quotation_Details.Add(new Models.Quotation_Details { Quotation_ID = last_inventory_id, Item_id = item_id[x], Qty = qty[x], Price = price[x], Batch = batch[x] });
            }
            db.SaveChanges();
            float total = 0;
            total = (float)db.Quotation_Details.Where(a => a.Quotation_ID == last_inventory_id).Select(a => a.Qty * a.Price).Sum();
            var p = db.tblQuotations.Where(f => f.Quotation_ID == last_inventory_id).FirstOrDefault();
            p.Total = total;
            db.SaveChanges();
            db.Database.CurrentTransaction.Commit();

            int divid = (int)1;
           
            //add log
            sitemethods.addlog("Add Data", "Add Sales Quotation # (" + last_inventory_id + ")", divid, userid);


            return RedirectToAction("Quotation", "Sales", new { id = last_inventory_id });
        }





        public ActionResult QuotationList()
        {

            //UsersController u = new UsersController();
            //u.checkaccess(35);
            return View();


        }

        public ActionResult SaleCashQuotation(int id)
        {
            var q = db.tblQuotations.Where(a => a.Quotation_ID == id).FirstOrDefault();
            if(q!= null)
            {
                db.tblInventory_Transactions.Add(new tblInventory_Transactions
                {
Business_Division_Id=q.Business_Division_Id,
Customer_ID=q.Customer_ID,
Total=q.Total,
Inventory_type_id=2,
Tran_Date=q.Tran_Date,
Tran_Nbr=q.Tran_Nbr,
UserID=q.UserID,
Discount=q.Discount,
Status="Open",
Payment_Status="Cash",
Payment_Method="Cash",
Marketer_id=q.Marketer_id

                });
                db.SaveChanges();

                int last_inventory_id = 0;
                last_inventory_id = db.tblInventory_Transactions.Select(a => a.Inventory_Tran_ID).Max();

                var qdetails = db.Quotation_Details.Where(a => a.Quotation_ID == id).ToList();


  foreach (var  x in qdetails)
                {
                    int itemid =(int) x.Item_id;
                    int qty = (int)x.Qty;
                    float price = (float)x.Price;
                    string batch = x.Batch;
                    var item_Detail = db.Items.Where(a => a.Item_Id == itemid).FirstOrDefault();
 db.tblInventory_Transaction_Details.Add(new Models.tblInventory_Transaction_Details { Inventory_Tran_ID = last_inventory_id, Item_Id = itemid, Qty_In = 0, Qty_Out = qty, Unit_Price = (float)item_Detail.Unit_Cost, Sales_price = price, Batch = batch });
                    db.SaveChanges();

                }

                q.Status = "Sold";
                db.SaveChanges();

                int divid = (int)1;
                int userid = (int)Session["UserID"];
                //add log
                sitemethods.addlog("Add Data", "Add Cash Sales Inv# (" + last_inventory_id + ") From Quotation #"+id, divid, userid);



                return RedirectToAction("Receipt", "Sales", new { id = last_inventory_id });


            }



            return View();
        }


        public ActionResult SaleCreditQuotation(int id,DateTime? trandate)
        {


            DateTime vdate = helper.sitemethods.getdate();
            if (trandate != null)
            {
                DateTime dt = (DateTime)trandate;
                if (dt.Date == vdate.Date)
                {
                    vdate = helper.sitemethods.getdate();
                }
                else
                {
                    vdate = dt;
                }
            }


            var q = db.tblQuotations.Where(a => a.Quotation_ID == id).FirstOrDefault();

            if (q.Customer_ID==null)
            {
                return RedirectToAction("error", "message", new { msg = "No Customer Asscociated with this Quotation" }); 

            }


            if (q != null)
            {
                db.tblInventory_Transactions.Add(new tblInventory_Transactions
                {
                    Business_Division_Id = q.Business_Division_Id,
                    Customer_ID = q.Customer_ID,
                    Total = q.Total,
                    Inventory_type_id = 2,
                    Tran_Date = q.Tran_Date,
                    Tran_Nbr = q.Tran_Nbr,
                    UserID = q.UserID,
                    Discount = q.Discount,
                    Status = "Open",
                    Payment_Status = "Invoice",
                    Payment_Method = "Invoice",
                    Marketer_id=q.Marketer_id

                });
                db.SaveChanges();

                int last_inventory_id = 0;
                last_inventory_id = db.tblInventory_Transactions.Select(a => a.Inventory_Tran_ID).Max();

                var qdetails = db.Quotation_Details.Where(a => a.Quotation_ID == id).ToList();


                foreach (var x in qdetails)
                {
                    int itemid = (int)x.Item_id;
                    int qty = (int)x.Qty;
                    float price = (float)x.Price;
                    string batch = x.Batch;
                    var item_Detail = db.Items.Where(a => a.Item_Id == itemid).FirstOrDefault();
                    db.tblInventory_Transaction_Details.Add(new Models.tblInventory_Transaction_Details { Inventory_Tran_ID = last_inventory_id, Item_Id = itemid, Qty_In = 0, Qty_Out = qty, Unit_Price = (float)item_Detail.Unit_Cost, Sales_price = price, Batch = batch });
                    db.SaveChanges();

                }

                q.Status = "Sold";
                db.SaveChanges();

                int divid = (int)1;
                int userid = (int)Session["UserID"];
                //add log

                sitemethods.addlog("Add Data", "Add Credit Sales Inv# (" + last_inventory_id + ") From Quotation #" + id, divid, userid);


               // add customer Transction
                    int Cust_Tran_Type = 0;
                    Cust_Tran_Type = db.tblCustomer_Tran_Type.Where(a => a.Tran_Type == "Purchase").Select(a => a.Cust_Tran_Type_ID).FirstOrDefault();
                    int? cust_tran_id = null;
                    try
                    {
                        cust_tran_id = db.tblCustomer_Transaction.Where(a => a.CustomerID == (int)q.Customer_ID && a.Status == "Open").Select(a => a.Cust_Tran_ID).Max();
                    }
                    catch (Exception ex) { }


                    if (cust_tran_id == null)
                    {
                        db.tblCustomer_Transaction.Add(new tblCustomer_Transaction { Amount = q.Total-q.Discount, CustomerID = (int)q.Customer_ID, Status = "Open", Tran_Date = DateTime.Now.Date, UserID = userid });
                        db.SaveChanges();
                        cust_tran_id = db.tblCustomer_Transaction.Where(a => a.UserID == userid).Select(a => a.Cust_Tran_ID).Max();
                    }
                    db.tblCustomer_Transaction_Detail.Add(new tblCustomer_Transaction_Detail { Debit = q.Total-q.Discount, Credit = 0, Cust_Tran_Type_ID = Cust_Tran_Type, Status = "Open", UserID = userid, Description = "Custmer Invoice Order #" + id, Inventory_Tran_ID = id, Tran_Nbr = "N/A", Trans_Date = vdate, Cust_Tran_ID = (int)cust_tran_id });
                    db.SaveChanges();

                    int trans_detail_id = db.tblCustomer_Transaction_Detail.Where(a => a.Cust_Tran_ID == cust_tran_id).Select(a => a.Cust_Transaction_Detail_id).FirstOrDefault();



                    db.Sp_Update_Customer_Trans_Total(cust_tran_id);



                    return RedirectToAction("Installment", "Sales", new { id = last_inventory_id, customer = q.Customer_ID });

                }
            



            return View();
        }





        public ActionResult MarketerSales(int? divid, DateTime? from, DateTime? to,int? marketer)

        {
            //UsersController u = new UsersController();
            //u.checkaccess(51);

            if (divid != null)
            {
                ViewBag.form = from;
                ViewBag.to = to;
                ViewBag.divid = divid;
                ViewBag.marketer = marketer;
            }
            return View();

        }



        public ActionResult Topsellingitems(DateTime? from,DateTime? to,int? divid)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(52);
            ViewBag.from = from;
            ViewBag.to = to;
            ViewBag.divid = divid;

            return View();
        }


        public ActionResult ItemSalesReport(DateTime? from, DateTime? to, int? divid,int? customer)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(53);

            ViewBag.from = from;
            ViewBag.to = to;
            ViewBag.divid = divid;
            ViewBag.customer = customer;
            return View();
        }


        public ActionResult ItemSalesReportByItem(DateTime? from, DateTime? to, int? divid, int? items)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(53);

            ViewBag.from = from;
            ViewBag.to = to;
            ViewBag.divid = divid;
            ViewBag.itemid = items;
            return View();
        }











        public ActionResult Update_Sales(int id)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(37);
            ViewBag.id = id;
            return View();
        }

        
             public ActionResult update_sales_order(int[] item_id, float[] qty, string[] batch, float[] price, float discount, string payment_method, int? customer_id, int order_id,DateTime? trandate, float? cash_payment)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(37);
            int userid = (int)Session["UserID"];
            int business_division = (int)1;

            if(cash_payment==null)
            { cash_payment = 0; }

            DateTime vdate = helper.sitemethods.getdate();
            if (trandate != null)
            {
                DateTime dt = (DateTime)trandate;
                if (dt.Date == vdate.Date)
                {
                    vdate = helper.sitemethods.getdate();
                }
                else
                {
                    vdate = dt;
                }
            }


            // Validate
            db.Database.BeginTransaction();
         var ord = db.tblInventory_Transactions.Where(a => a.Inventory_Tran_ID == order_id).FirstOrDefault();
            if (ord != null)
            {

                if(ord.Status.Contains("Open")==false)
                {
                    return RedirectToAction("error", "message", new { msg = "Posred or Cancelled Order Could not be Updated " });

                }

            }
            else { return RedirectToAction("error", "message", new { msg = "Invalid Order " }); }

            

            // Delete old customer transctions

           var tran= db.tblCustomer_Transaction_Detail.Where(a => a.Cust_Transaction_Detail_id == order_id).FirstOrDefault();
            int tran_id = tran.Cust_Tran_ID;
            if (tran!=null)
            {
                db.tblCustomer_Transaction_Detail.Remove(tran);
                db.SaveChanges();
            }
            db.Sp_Update_Customer_Trans_Total(tran_id);

            // Delete Installment Plan

            IEnumerable<InstallmentPlan> i = db.InstallmentPlans.Where(a => a.OrderId == order_id).ToList();
            db.InstallmentPlans.RemoveRange(i);
            db.SaveChanges();




            // Delete Prev. Order Details

            IEnumerable<tblInventory_Transaction_Details> old_details = db.tblInventory_Transaction_Details.Where(a => a.Inventory_Tran_ID == order_id).ToList();
            db.tblInventory_Transaction_Details.RemoveRange(old_details);
            db.SaveChanges();

  int last_inventory_id = 0;
  last_inventory_id = order_id;
  for (int x = 0; x <= item_id.Length - 1; x++)
            {
                int itemid = item_id[x];
                var item_Detail = db.Items.Where(a => a.Item_Id == itemid).FirstOrDefault();
                db.tblInventory_Transaction_Details.Add(new Models.tblInventory_Transaction_Details { Inventory_Tran_ID = last_inventory_id, Item_Id = item_id[x], Qty_In = 0, Qty_Out = qty[x], Unit_Price = (float)item_Detail.Unit_Cost, Sales_price = price[x], Batch = batch[x] });
            }
            db.SaveChanges();
            float total = 0;
            total = (float)db.tblInventory_Transaction_Details.Where(a => a.Inventory_Tran_ID == last_inventory_id).Select(a => a.Qty_Out * a.Sales_price).Sum();
            var p = db.tblInventory_Transactions.Where(f => f.Inventory_Tran_ID == last_inventory_id).FirstOrDefault();

            if (payment_method.Contains("Invoice"))
            {
            }
            else
            {
                p.Cash_Payment = total;
            }

            float oldtotal = p.Total;
            float olddiscount = p.Discount;
            int? oldcust = p.Customer_ID;
            p.Total = total;
            p.Tran_Date = vdate;
            p.Customer_ID = customer_id;
            p.Discount = discount;
            p.UserID = userid;
            p.Cash_Payment = cash_payment;

           // int serialno = db.tblInventory_Transactions.Where(a => a.Business_Division_Id == business_division && a.Inventory_type_id == inventory_type_id).Select(a => a.Inventory_Tran_ID).Count();
           //  p.Invoice_No = serialno;

            db.SaveChanges();
            db.Database.CurrentTransaction.Commit();

            //add log
            sitemethods.addlog("Update Data", "Updated Sales Inv # (" + p.Invoice_No + ") Customer # "+oldcust +" Total was  " + oldtotal + " Discount was " + olddiscount + " ;   New Invoice For Customer #"+customer_id+" Total = " + total+" and Discount ="+discount, business_division, userid);




            // Add Customer Invoice

            if (payment_method.Contains("Invoice"))
            {

                int Cust_Tran_Type = 0;
                Cust_Tran_Type = db.tblCustomer_Tran_Type.Where(a => a.Tran_Type == "Purchase").Select(a => a.Cust_Tran_Type_ID).FirstOrDefault();
                int? cust_tran_id = null;
                try
                {
                    cust_tran_id = db.tblCustomer_Transaction.Where(a => a.CustomerID == customer_id && a.Status == "Open").Select(a => a.Cust_Tran_ID).Max();
                }
                catch (Exception ex) { }


                if (cust_tran_id == null)
                {
                    db.tblCustomer_Transaction.Add(new tblCustomer_Transaction { Amount = total-discount, CustomerID = (int)customer_id, Status = "Open", Tran_Date = vdate, UserID = userid });
                    db.SaveChanges();
                    cust_tran_id = db.tblCustomer_Transaction.Where(a => a.UserID == userid).Select(a => a.Cust_Tran_ID).Max();
                }
                db.tblCustomer_Transaction_Detail.Add(new tblCustomer_Transaction_Detail { Debit = total - discount, Credit = 0, Cust_Tran_Type_ID = Cust_Tran_Type, Status = "Open", UserID = userid, Description = "Custmer Invoice Order #" + p.Invoice_No, Inventory_Tran_ID = order_id, Tran_Nbr = "N/A", Trans_Date = vdate, Cust_Tran_ID = (int)cust_tran_id });
                db.SaveChanges();

                int trans_detail_id = db.tblCustomer_Transaction_Detail.Where(a => a.Cust_Tran_ID == cust_tran_id).Select(a => a.Cust_Transaction_Detail_id).FirstOrDefault();
                db.Sp_Update_Customer_Trans_Total(cust_tran_id);
                return RedirectToAction("Installment", "Sales", new { id = last_inventory_id, customer = customer_id });

            }

            return RedirectToAction("Receipt", "Sales", new { id = last_inventory_id });
        }




        public ActionResult updateinvoiceNumber(int id,int? invoice)
        {
            int userid = (int)Session["UserID"];
            int divid = (int)1;
            var inv = db.tblInventory_Transactions.Where(a => a.Inventory_Tran_ID == id).FirstOrDefault();
            if(inv!=null)
            {
            

                sitemethods.addlog("Update Data", "Sales  Transction # " + id + " Invoice # updated from " + inv.Invoice_No + " to " + invoice, divid, userid);
                inv.Invoice_No = invoice;
                db.SaveChanges();
                return RedirectToAction("UpdateSales", "sales", new { msg = "Invoice id " + id + " Updated to " + invoice });

            }
            return RedirectToAction("UpdateSales", "sales", new { msg = "Invoice id " + id + " Not Updated" });
               }

    }

}
