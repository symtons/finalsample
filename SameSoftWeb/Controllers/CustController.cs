using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SameSoftWeb.Models;
using SameSoftWeb.helper;

namespace Safari_Web_Application.Controllers
{
    public class CustController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        public ActionResult CustomerBalance()
        {
        //    UsersController u = new UsersController();
        //    u.checkaccess(11);

            return View();
        }
        public ActionResult CustomerStatement(int? id)
        {
            ViewBag.id = id;

            return View();
        }

        public ActionResult CustomerTransaction(int? id,DateTime? from, DateTime? to, int? type)
        {
            ViewBag.from = from;
            ViewBag.to = to;
            ViewBag.type = type;
            ViewBag.id = id;

            return View();
        }
        public ActionResult CustomerTrans(int? id)
        {
            ViewBag.id = id;

            return View();
        }
        //public ActionResult CustomerTrans(int? id,DateTime? from,DateTime? to)
        //{
        //    //UsersController u = new UsersController();
        //    //u.checkaccess(11);

        //    ViewBag.from = from;
        //    ViewBag.to = to;
        //    ViewBag.id = id;

        //    return View();
        //}
        [HttpPost]
        public ActionResult CustomerPayment(int customerid, int trantype, float amount, string desc, int? cashacc, int? discountacc, int? aracc,DateTime trans_Date)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(11);

            string tranNbr = DateTime.UtcNow.AddHours(3).ToString().GetHashCode().ToString("x");

            int userid = (int)Session["UserID"];



            string tranname = "";
            if (trantype == 1) { tranname = "Customer Payment"; }
            else if (trantype == 2) { tranname = "Customer Discount"; } else { tranname = "Customer Refund"; }

          

            int Cust_Tran_Type = trantype;
            DateTime vdate = trans_Date;
            string customername = "";
            customername = db.tblCustomers.Where(a => a.CustomerID == customerid).Select(a => a.Customer_Name).FirstOrDefault();

            string inwords = "";
            int? voucher_id;

            int? cust_tran_id = db.tblCustomer_Transaction.Where(a => a.CustomerID == customerid && a.Status == "Open").Select(a => a.Cust_Tran_ID).Max();



            //add log
            mnc.addlog("Customers", " " + tranname + " | Customer " + customername +" Amount: " + amount, userid);


            if (cust_tran_id == null)
            {
                db.tblCustomer_Transaction.Add(new tblCustomer_Transaction { Amount = amount, CustomerID = customerid, Status = "Open", Tran_Date = vdate, UserID = userid });
                db.SaveChanges();
                cust_tran_id = db.tblCustomer_Transaction.Where(a => a.UserID == userid).Select(a => a.Cust_Tran_ID).Max();
            }
            if (trantype == 1)
            {
                db.tblCustomer_Transaction_Detail.Add(new tblCustomer_Transaction_Detail { Debit = 0, Credit = amount, Cust_Tran_Type_ID = Cust_Tran_Type, Status = "Open", UserID = userid, Description = "Custmer Payment - " + desc, Tran_Nbr = tranNbr, Trans_Date = vdate, Cust_Tran_ID = (int)cust_tran_id });
            }
            if (trantype == 2)
            {
                db.tblCustomer_Transaction_Detail.Add(new tblCustomer_Transaction_Detail { Debit = 0, Credit = amount, Cust_Tran_Type_ID = Cust_Tran_Type, Status = "Open", UserID = userid, Description = "Custmer Discount -" + desc, Tran_Nbr = tranNbr, Trans_Date = vdate, Cust_Tran_ID = (int)cust_tran_id });

            }


            if (trantype == 3)
            {
                db.tblCustomer_Transaction_Detail.Add(new tblCustomer_Transaction_Detail { Debit = amount, Credit = 0, Cust_Tran_Type_ID = Cust_Tran_Type, Status = "Open", UserID = userid, Description = "Custmer Refund -" + desc, Tran_Nbr = tranNbr, Trans_Date = vdate, Cust_Tran_ID = (int)cust_tran_id });

            }
            db.SaveChanges();


            float intTotal_Vendor_Balance = (float)db.Get_Total_Customer_Transaction_Detail_by_TranID(cust_tran_id).FirstOrDefault();
            db.Update_Customer_Tranaction_Amount_By_Cust_tran_id(intTotal_Vendor_Balance, cust_tran_id);



            // Add Journal
            tblJournal j = new tblJournal();

            j.Journal_Date = vdate;
            j.Tran_Nbr = tranNbr;
            j.TYear = DateTime.UtcNow.AddHours(3).Year;
            j.Period = DateTime.UtcNow.AddHours(3).Month;
            j.UserID = userid;
            j.Status = "Open";
            j.Tran_Type = "Customers";
            j.Description = "Customer Transaction-" + desc;
            db.tblJournals.Add(j);
            db.SaveChanges();
            int Journal_id = db.tblJournals.Where(a => a.UserID == userid).Select(a => a.Journal_id).Max();

            if (trantype == 1)
            {
                // Cash
                db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = (int)cashacc });
                db.SaveChanges();
                // Add cash flow 
                int Journal_Detail_id = db.tblJournal_Details.Where(a => a.Journal_id == Journal_id).Select(a => a.Journal_Detail_id).Max();
                db.tblCash_Flow.Add(new tblCash_Flow { Debit = amount, Credit = 0, Journal_Detail_id = Journal_Detail_id, Description = "Cash Recieved from CustomerID " + customerid, Tran_Nbr = tranNbr, Tran_Type = "Cash In" });
                // Acc Receiable
                db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = (int)aracc });
                db.SaveChanges();
            }

            if (trantype == 2)
            {
                // Discount Expense
                db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = (int)discountacc });
                // Acc Receivable
                db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = (int)aracc });
                db.SaveChanges();

            }

            if (trantype == 3)
            {
                // Cash
                db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = (int)cashacc });
                db.SaveChanges();
                // Add cash flow 
                int Journal_Detail_id = db.tblJournal_Details.Where(a => a.Journal_id == Journal_id).Select(a => a.Journal_Detail_id).Max();
                db.tblCash_Flow.Add(new tblCash_Flow { Debit = 0, Credit = amount, Journal_Detail_id = Journal_Detail_id, Description = "Cash Refunded for Customer " + customername, Tran_Nbr = tranNbr, Tran_Type = "Cash Out" });
                // Acc Receiable
                db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = (int)aracc });
                db.SaveChanges();
            }

          if (trantype == 1)
            {
                string strWord = mnc.ToWords(amount.ToString());
                var x = db.tblReceipt_Voucher.Add(new tblReceipt_Voucher { Receipt_Date = vdate, Balance= intTotal_Vendor_Balance, Tran_Nbr = tranNbr, Description = "Customer Payment", Payee = customername, UserID = userid, Amount = amount, In_Word = strWord });
                db.SaveChanges();

                voucher_id = db.tblReceipt_Voucher.Where(a => a.UserID == userid).Select(a => a.Voucher_ID).Max();
                //  UpdateVoucher_Number((int)voucher_id);
                return RedirectToAction("ReceiptVoucher", "Accounting", new { id = voucher_id });
            }



            if (trantype == 3)
            {
                string strWord = mnc.ToWords(amount.ToString());

                var x = db.tblPayment_Voucher.Add(new tblPayment_Voucher { Payment_Date = vdate, Tran_Nbr = tranNbr, Description = "Customer Refund", Payee = customername, UserID = userid,  Amount = amount, In_Word = strWord });
                db.SaveChanges();

                voucher_id = db.tblPayment_Voucher.Where(a => a.UserID == userid).Select(a => a.Voucher_ID).Max();
                // UpdateVoucher_Number((int)voucher_id);
                return RedirectToAction("PaymentVoucher", "Accounting", new { id = voucher_id });
            }


            return RedirectToAction("CustomerTrans", "Cust", new { id = customerid, msg = "Customer Transaction Recorded Successfully" });


        }














        public ActionResult InvoiceGroups(int? customer,string msg, int? invoice_id)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(14);

            ViewBag.id = customer;
            ViewBag.msg = msg;
            ViewBag.invoice_id = invoice_id;
            return View();
        }

        public ActionResult CreateInvoiceGroup(int customer_id,int invoice_number,string title,string desc,DateTime date)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(14);

            int UserID = (int)Session["UserID"];

            var inv = db.tblCustomerInvoiceGroups.Where(a => a.Customer_ID == customer_id && a.Invoice_Number == invoice_number).FirstOrDefault();
            if(inv!=null)
            {
                return RedirectToAction("InvoiceGroups", "Cust", new { customer = customer_id, msg = "Invoice Number Already Exist" });
            }
              
            var custName = db.tblCustomers.Where(a => a.CustomerID == customer_id).Select(a => a.Customer_Name).First();


          db.tblCustomerInvoiceGroups.Add(new tblCustomerInvoiceGroup {Customer_ID=customer_id,Title=title,Description=desc,Invoice_Date=date,UserID=UserID,Invoice_Number=invoice_number,Last_Update=DateTime.UtcNow.AddHours(3).Date });
            db.SaveChanges();

             inv = db.tblCustomerInvoiceGroups.Where(a => a.Customer_ID == customer_id && a.Invoice_Number == invoice_number).FirstOrDefault();

            var i = db.tblCustomerInvoiceGroups.Select(a => a.Invoice_Group_Id).Max();

            mnc.addlog("Customers", "Invoice #"+inv.Invoice_Number +" Created for  "+custName, (int)Session["UserID"]);

            return RedirectToAction("InvoiceGroups", "Cust", new { customer = customer_id, msg = "Invoice Created Successfully",invoice_id=i });


         
        }

        public ActionResult AddInvoiceTransctions(int customer,int invoice_id , int? selected_cust_tran_id)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(14);

            ViewBag.invoice_id=invoice_id;
            ViewBag.customer = customer;
            ViewBag.selected_cust_tran_id = selected_cust_tran_id;
            return View();
        }


        public ActionResult addInvoiceTrans(int[] selectedtrans, int invoice_id, float[] qty)
        {

            //UsersController u = new UsersController();
            //u.checkaccess(14);

            for (int x=0;x<=selectedtrans.Length-1;x++)
            {
                int tran_id = selectedtrans[x];
                var cust_Tran_rc = db.tblCustomer_Transaction_Detail.Where(a => a.Cust_Transaction_Detail_id == tran_id).FirstOrDefault();
                //string descr = "";
                //descr = desc;
                float qtyx = qty[x];
                //if (cust_Tran_rc!=null)
                //{
                //    cust_Tran_rc.Description = descr;
                //    db.SaveChanges();
                //}


                db.tblInvoiceGroupDetails.Add(new tblInvoiceGroupDetail { Invoice_Group_Id = invoice_id, Cust_Transaction_Detail_id = selectedtrans[x],QTY=qtyx });
                int id = selectedtrans[x];

                var detail = db.tblCustomer_Transaction_Detail.Where(a => a.Cust_Transaction_Detail_id == id).FirstOrDefault();
                detail.Invoice_Group_Id =invoice_id;

                db.SaveChanges();
            }


            var cust = db.tblCustomerInvoiceGroups.Where(a => a.Invoice_Group_Id == invoice_id).Select(a => a.Customer_ID).FirstOrDefault();
            var cust_name = db.tblCustomers.Where(a => a.CustomerID == cust).Select(a => a.Customer_Name).FirstOrDefault();


            var inv = db.tblCustomerInvoiceGroups.Where(a => a.Invoice_Group_Id == invoice_id).FirstOrDefault();
            inv.Last_Update = mnc.getdate();
            inv.Updated_by = (int)Session["UserID"];
            db.SaveChanges();

            mnc.addlog("Customers", selectedtrans.Length+ " Transactions added to Invoice #" + inv.Invoice_Number + "  for Customer  " + cust_name, (int)Session["UserID"]);

            return RedirectToAction("InvoiceGroups", "Cust", new { customer = inv.Customer_ID, msg = "Transaction added Successfully", invoice_id = invoice_id });

        }

 
        public ActionResult RemoveTranFromInvoice(int id,int tranid)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(14);

            var invdt = db.tblInvoiceGroupDetails.Where(a => a.Invoice_Group_Id == id && a.Cust_Transaction_Detail_id == tranid).FirstOrDefault();
            if(invdt!=null)
            {
                db.tblInvoiceGroupDetails.Remove(invdt);

                var dt = db.tblCustomer_Transaction_Detail.Where(a => a.Cust_Transaction_Detail_id == tranid).FirstOrDefault();
                dt.Invoice_Group_Id = null;
                db.SaveChanges();
                 
            }

           var inv = db.tblCustomerInvoiceGroups.Where(a => a.Invoice_Group_Id == id).FirstOrDefault();

            var cust = db.tblCustomerInvoiceGroups.Where(a => a.Invoice_Group_Id == id).Select(a => a.Customer_ID).FirstOrDefault();
            var cust_name = db.tblCustomers.Where(a => a.CustomerID == cust).Select(a => a.Customer_Name).FirstOrDefault();

            mnc.addlog("Customers"," Transaction # "+tranid+" Removed from Invoiced #" + inv.Invoice_Number + " for Customer " + cust_name, (int)Session["UserID"]);

            return RedirectToAction("InvoiceGroups", "Cust", new { customer = inv.Customer_ID, msg = "Transaction Removed Successfully", invoice_id = id });

        }


        

                  public ActionResult CloseTransaction(int id)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(14);

            var inv = db.tblCustomer_Transaction.Where(a => a.CustomerID == id && a.Status == "Open").FirstOrDefault();
            if(inv!=null)
            {
                if(inv.Amount==0)
                {
                    inv.Status = "Closed";
                    db.SaveChanges();
                    var cust = db.tblCustomer_Transaction.Where(a => a.Cust_Tran_ID == id).Select(a => a.CustomerID).FirstOrDefault();
                    var cust_name = db.tblCustomers.Where(a => a.CustomerID == cust).Select(a => a.Customer_Name).FirstOrDefault();

                    mnc.addlog("Customers", " Customer Transaction # " + id + " closed for customer " + cust_name, (int)Session["UserID"]);

                }
            }

  return RedirectToAction("CustomerBalance", "Cust", new { msg = "Transaction Closed Successfully"});

        }



        public ActionResult UpdateInvoice(int id, int invoice_num,string title,string desc,DateTime date,int customer)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(14);


            var inv = db.tblCustomerInvoiceGroups.Where(a => a.Invoice_Number == invoice_num && a.Customer_ID==customer).FirstOrDefault();

            if(inv!=null)
            {
if(inv.Invoice_Group_Id!=id)
                {
  return RedirectToAction("InvoiceGroups", "Cust", new { customer = inv.Customer_ID, msg = "Selected Invoice Number is already in use.", invoice_id = id });
               }
            }

            var inv2 = db.tblCustomerInvoiceGroups.Where(a => a.Invoice_Group_Id == id).FirstOrDefault();
            if(inv2!=null)
            {
                inv2.Invoice_Number = invoice_num;
                inv2.Title = title;
                inv2.Description = desc;
                inv2.Invoice_Date = date;
                inv2.Last_Update = mnc.getdate();
                inv2.Updated_by = (int)Session["UserID"];
                db.SaveChanges();
            }

            var cust = db.tblCustomerInvoiceGroups.Where(a => a.Invoice_Group_Id == id).Select(a => a.Customer_ID).FirstOrDefault();
            var cust_name = db.tblCustomers.Where(a => a.CustomerID == cust).Select(a => a.Customer_Name).FirstOrDefault();

            mnc.addlog("Cust", "Customer Invoice  # " + id + " Updated for customer " + cust_name, (int)Session["UserID"]);


            return RedirectToAction("InvoiceGroups", "Cust", new { customer = inv.Customer_ID, msg = "Invoice Updated Successfully", invoice_id = id });

        }



        public ActionResult PrintInvoice(int id)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(14);

            ViewBag.id = id;
            return View();
        }


        public ActionResult DeleteTransction(int id)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(14);

            int tranidd = 0;

            int customer_id = 0;
            var tran_detail = db.tblCustomer_Transaction_Detail.Where(a => a.Cust_Transaction_Detail_id == id).FirstOrDefault();
            if(tran_detail!=null)
            {
                tranidd = tran_detail.Cust_Tran_ID;

                string trannbr = tran_detail.Tran_Nbr;
                var journal = db.tblJournals.Where(a => a.Tran_Nbr == trannbr).FirstOrDefault();
                if(journal!=null)
                {

                     customer_id = db.tblCustomer_Transaction.Where(a => a.Cust_Tran_ID == tran_detail.Cust_Tran_ID).Select(a => a.CustomerID).FirstOrDefault();


                    string customername = db.tblCustomers.Where(a => a.CustomerID == customer_id).Select(a => a.Customer_Name).FirstOrDefault();


                    mnc.addlog("Customers", " Customer Transaction Details # " + id + "Deleted for Customer "+ customername + "  ; Description " + journal.Description, (int)Session["UserID"]);


                    db.tblJournals.Remove(journal);
                    db.SaveChanges();

                    db.tblCustomer_Transaction_Detail.Remove(tran_detail);
                    db.SaveChanges();

                    float intTotal_Vendor_Balance = (float)db.Get_Total_Customer_Transaction_Detail_by_TranID(tranidd).FirstOrDefault();
                    db.Update_Customer_Tranaction_Amount_By_Cust_tran_id(intTotal_Vendor_Balance, tranidd);


                }
            }



            return RedirectToAction("CustomerTrans", "Cust", new { id = customer_id, msg = "Customer Transaction Deleted Successfully" });

        }

        public ActionResult CustomerDeposit()
        {

            //UsersController u = new UsersController();
            //u.checkaccess(79);

            return View();
        }

        public ActionResult CustomerPrepaid()
        {
            //UsersController u = new UsersController();
            //u.checkaccess(79);
            return View();
        }

        public ActionResult DepositWithdrawel()
        {
            //UsersController u = new UsersController();
            //u.checkaccess(80);

            return View();
        }

        

public ActionResult AddCustomerWithdrawel(int customer, float amount, string desc, int cashacc, DateTime trandate)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(80);

            int depositpayableacc = 2115;
            int userid = (int)Session["UserID"];
            DateTime vdate = trandate;

            int Cust_Tran_Type = 7; // Withdrawel


            // Add Journal
            tblJournal j = new tblJournal();
            string customername = "";
            customername = db.tblCustomers.Where(a => a.CustomerID == customer).Select(a => a.Customer_Name).FirstOrDefault();

            string tranNbr = DateTime.UtcNow.AddHours(3).ToString().GetHashCode().ToString("x");
            j.Journal_Date = vdate;
            j.Tran_Nbr = tranNbr;
            j.TYear = DateTime.UtcNow.AddHours(3).Year;
            j.Period = DateTime.UtcNow.AddHours(3).Month;
            j.UserID = userid;
            j.Status = "Open";
            j.Tran_Type = "Withdrawel ";
            j.Description = "Withdrawel " + customername + "-" + desc;
            db.tblJournals.Add(j);
            db.SaveChanges();
            int Journal_id = db.tblJournals.Where(a => a.UserID == userid).Select(a => a.Journal_id).Max();
            // Deposit Payable
            db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = depositpayableacc });
            db.SaveChanges();

            // Cash
            db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = cashacc });
            db.SaveChanges();
            // Add cash flow 
            int Journal_Detail_id = db.tblJournal_Details.Where(a => a.Journal_id == Journal_id).Select(a => a.Journal_Detail_id).Max();
            db.tblCash_Flow.Add(new tblCash_Flow { Debit = 0, Credit = amount, Journal_Detail_id = Journal_Detail_id, Description = "Cash Paid from Customer Withdrawel  " + customername, Tran_Nbr = tranNbr, Tran_Type = "Cash Out" });

            // add customer Transaction
            int? cust_tran_id = db.tblCustomer_Transaction.Where(a => a.CustomerID == customer && a.Status == "Open").Select(a => a.Cust_Tran_ID).FirstOrDefault();

            if (cust_tran_id == null || cust_tran_id == 0)
            {
                db.tblCustomer_Transaction.Add(new tblCustomer_Transaction { Amount = amount, CustomerID = customer, Status = "Open", Tran_Date = vdate, UserID = userid });
                db.SaveChanges();
                cust_tran_id = db.tblCustomer_Transaction.Where(a => a.UserID == userid).Select(a => a.Cust_Tran_ID).FirstOrDefault();
            }

            db.tblCustomer_Transaction_Detail.Add(new tblCustomer_Transaction_Detail { Debit = amount, Credit = 0, Cust_Tran_Type_ID = Cust_Tran_Type, Status = "Open", UserID = userid, Description = "Custmer Withdrawel - " + desc, Tran_Nbr = tranNbr, Trans_Date = vdate, Cust_Tran_ID = (int)cust_tran_id });
            db.SaveChanges();


            float intTotal_Vendor_Balance = (float)db.Get_Total_Customer_Transaction_Detail_by_TranID(cust_tran_id).FirstOrDefault();
            db.Update_Customer_Tranaction_Amount_By_Cust_tran_id(intTotal_Vendor_Balance, cust_tran_id);



            //add log
            mnc.addlog("Customers", " Customer Withdrawel  " + customername + " Amount: " + amount, userid);


           


            string strWord = mnc.ToWords(amount.ToString());

            var x = db.tblPayment_Voucher.Add(new tblPayment_Voucher { Payment_Date = vdate, Tran_Nbr = tranNbr, Description = "Customer Withdrawel", Payee = customername, UserID = userid, Amount = amount, In_Word = strWord });
            db.SaveChanges();

            int voucher_id = db.tblPayment_Voucher.Where(a => a.UserID == userid).Select(a => a.Voucher_ID).Max();
            // UpdateVoucher_Number((int)voucher_id);
            return RedirectToAction("PaymentVoucher", "Accounting", new { id = voucher_id });


            return View();
        }









        public ActionResult AddCustomerDeposit(int customer, float amount, string desc, int cashacc, DateTime trandate)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(79);

            int depositpayableacc = 2115;
            int userid = (int)Session["UserID"];
            DateTime vdate = trandate;

            int Cust_Tran_Type = 5; // Deposit


            // Add Journal
            tblJournal j = new tblJournal();
            string customername = "";
            customername = db.tblCustomers.Where(a => a.CustomerID == customer).Select(a => a.Customer_Name).FirstOrDefault();

            string tranNbr = DateTime.UtcNow.AddHours(3).ToString().GetHashCode().ToString("x");
            j.Journal_Date = vdate;
            j.Tran_Nbr = tranNbr;
            j.TYear = DateTime.UtcNow.AddHours(3).Year;
            j.Period = DateTime.UtcNow.AddHours(3).Month;
            j.UserID = userid;
            j.Status = "Open";
            j.Tran_Type = "Customer Deposit";
            j.Description = "Deposit " + customername+"-"+desc; 
            db.tblJournals.Add(j);
            db.SaveChanges();
            int Journal_id = db.tblJournals.Where(a => a.UserID == userid).Select(a => a.Journal_id).Max();
            // Deposit Payable
            db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = depositpayableacc });
            db.SaveChanges();

            // Cash
            db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = cashacc });
            db.SaveChanges();
            // Add cash flow 
            int Journal_Detail_id = db.tblJournal_Details.Where(a => a.Journal_id == Journal_id).Select(a => a.Journal_Detail_id).Max();
            db.tblCash_Flow.Add(new tblCash_Flow { Debit = amount, Credit = 0, Journal_Detail_id = Journal_Detail_id, Description = "Cash Received from Customer Deposit  " + customername, Tran_Nbr = tranNbr, Tran_Type = "Cash In" });

            // add customer Transaction
            int? cust_tran_id = db.tblCustomer_Transaction.Where(a => a.CustomerID == customer && a.Status == "Open").Select(a => a.Cust_Tran_ID).FirstOrDefault();

            if (cust_tran_id == null || cust_tran_id==0)
            {
                db.tblCustomer_Transaction.Add(new tblCustomer_Transaction { Amount = amount, CustomerID = customer, Status = "Open", Tran_Date = vdate, UserID = userid });
                db.SaveChanges();
                cust_tran_id = db.tblCustomer_Transaction.Where(a => a.UserID == userid).Select(a => a.Cust_Tran_ID).Max();
            }
           
                db.tblCustomer_Transaction_Detail.Add(new tblCustomer_Transaction_Detail { Debit = 0, Credit = amount, Cust_Tran_Type_ID = Cust_Tran_Type, Status = "Open", UserID = userid, Description = "Custmer Deposit - " + desc, Tran_Nbr = tranNbr, Trans_Date = vdate, Cust_Tran_ID = (int)cust_tran_id });
                db.SaveChanges();


            float intTotal_Vendor_Balance = (float)db.Get_Total_Customer_Transaction_Detail_by_TranID(cust_tran_id).FirstOrDefault();
            db.Update_Customer_Tranaction_Amount_By_Cust_tran_id(intTotal_Vendor_Balance, cust_tran_id);



            //add log
            mnc.addlog("Customers", " Customer Deposit  " + customername + " Amount: " + amount, userid);



            string strWord = mnc.ToWords(amount.ToString());
            var x = db.tblReceipt_Voucher.Add(new tblReceipt_Voucher { Receipt_Date = vdate, Balance = intTotal_Vendor_Balance, Tran_Nbr = tranNbr, Description = "Customer Deposit", Payee = customername, UserID = userid, Amount = amount, In_Word = strWord });
            db.SaveChanges();

            int voucher_id = db.tblReceipt_Voucher.Where(a => a.UserID == userid).Select(a => a.Voucher_ID).Max();
            //  UpdateVoucher_Number((int)voucher_id);
            return RedirectToAction("ReceiptVoucher", "Accounting", new { id = voucher_id });




        }












        public ActionResult DeleteInvoiceGroup(int id)
        {

            //UsersController u = new UsersController();
            //u.checkaccess(14);

            var inv = db.tblCustomerInvoiceGroups.Where(a => a.Invoice_Group_Id == id).FirstOrDefault();
            if (inv != null)
            {
                var details = db.tblInvoiceGroupDetails.Where(a => a.Invoice_Group_Id == inv.Invoice_Group_Id).ToList();
                if (details != null)
                {
                    db.tblInvoiceGroupDetails.RemoveRange(details);
                    db.SaveChanges();
                    db.tblCustomerInvoiceGroups.Remove(inv);
                    db.SaveChanges();
                }

                return Content("Invoice Group Deleted");
            }
            return Content("Nothing Deleted");
        }












        public ActionResult AddCustomerPrepaid(int customer, float amount, string desc, int cashacc, DateTime trandate)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(79);

            int depositpayableacc = 2110;
            int userid = (int)Session["UserID"];
            DateTime vdate = trandate;

            int Cust_Tran_Type = 8; // Prepaid


            // Add Journal
            tblJournal j = new tblJournal();
            string customername = "";
            customername = db.tblCustomers.Where(a => a.CustomerID == customer).Select(a => a.Customer_Name).FirstOrDefault();

            string tranNbr = DateTime.UtcNow.AddHours(3).ToString().GetHashCode().ToString("x");
            j.Journal_Date = vdate;
            j.Tran_Nbr = tranNbr;
            j.TYear = DateTime.UtcNow.AddHours(3).Year;
            j.Period = DateTime.UtcNow.AddHours(3).Month;
            j.UserID = userid;
            j.Status = "Open";
            j.Tran_Type = "Customer Prepaid";
            j.Description = "Prepaid " + customername + "-" + desc;
            db.tblJournals.Add(j);
            db.SaveChanges();
            int Journal_id = db.tblJournals.Where(a => a.UserID == userid).Select(a => a.Journal_id).Max();
            // Deposit Payable
            db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = depositpayableacc });
            db.SaveChanges();

            // Cash
            db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = cashacc });
            db.SaveChanges();
            // Add cash flow 
            int Journal_Detail_id = db.tblJournal_Details.Where(a => a.Journal_id == Journal_id).Select(a => a.Journal_Detail_id).Max();
            db.tblCash_Flow.Add(new tblCash_Flow { Debit = amount, Credit = 0, Journal_Detail_id = Journal_Detail_id, Description = "Cash Received from Customer Prepaid  " + customername, Tran_Nbr = tranNbr, Tran_Type = "Cash In" });

            // add customer Transaction
            int? cust_tran_id = db.tblCustomer_Transaction.Where(a => a.CustomerID == customer && a.Status == "Open").Select(a => a.Cust_Tran_ID).FirstOrDefault();

            if (cust_tran_id == null || cust_tran_id == 0)
            {
                db.tblCustomer_Transaction.Add(new tblCustomer_Transaction { Amount = amount, CustomerID = customer, Status = "Open", Tran_Date = vdate, UserID = userid });
                db.SaveChanges();
                cust_tran_id = db.tblCustomer_Transaction.Where(a => a.UserID == userid).Select(a => a.Cust_Tran_ID).Max();
            }

            db.tblCustomer_Transaction_Detail.Add(new tblCustomer_Transaction_Detail { Debit = 0, Credit = amount, Cust_Tran_Type_ID = Cust_Tran_Type, Status = "Open", UserID = userid, Description = "Custmer Prepaid - " + desc, Tran_Nbr = tranNbr, Trans_Date = vdate, Cust_Tran_ID = (int)cust_tran_id });
            db.SaveChanges();


            float intTotal_Vendor_Balance = (float)db.Get_Total_Customer_Transaction_Detail_by_TranID(cust_tran_id).FirstOrDefault();
            db.Update_Customer_Tranaction_Amount_By_Cust_tran_id(intTotal_Vendor_Balance, cust_tran_id);



            //add log
            mnc.addlog("Customers", " Customer Prepaid  " + customername + " Amount: " + amount, userid);



            string strWord = mnc.ToWords(amount.ToString());
            var x = db.tblReceipt_Voucher.Add(new tblReceipt_Voucher { Receipt_Date = vdate, Balance = intTotal_Vendor_Balance, Tran_Nbr = tranNbr, Description = "Customer Prepaid", Payee = customername, UserID = userid, Amount = amount, In_Word = strWord });
            db.SaveChanges();

            int voucher_id = db.tblReceipt_Voucher.Where(a => a.UserID == userid).Select(a => a.Voucher_ID).Max();
            //  UpdateVoucher_Number((int)voucher_id);
            return RedirectToAction("ReceiptVoucher", "Accounting", new { id = voucher_id });




        }
    }
}
