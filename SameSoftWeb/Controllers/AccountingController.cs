using SameSoftWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SameSoftWeb.Controllers
{
    public class AccountingController : Controller
    {
        // GET: Accounting

        SameSoftWeb.Models.SameSoftwareWebEntities db = new Models.SameSoftwareWebEntities();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AddCustomer(string name)
        {
            ViewBag.name = name;
            return View();
        }

        public ActionResult PaymentVoucher(int id)
        {

            ViewBag.id = id;
            return View();
        }


        public ActionResult ReceiptVoucher(int id)
        {

            ViewBag.id = id;
            return View();
        }
        public ActionResult AddInventory()
        {
            //UsersController u = new UsersController();
            //u.checkaccess(12);
            return View();

        }

        public ActionResult AddManualInventory(int? creditpayableacc, float? freight_rate, float? tax_rate, int[] item_id, int? creditinvacc, int? cashinvacc, int? assetinvacc, int? cashVendor_id, int? Creditvendor_id, int? cashacc, int? asset, float[] qty, float[] unit_cost, float[] price, string[] lot, string[] barcode, DateTime?[] productdate, DateTime?[] expiredate, string desc, int paymenttype)
        {

            string tranNbr = DateTime.UtcNow.ToString();
            int divid = 1;
            int userid = (int)Session["UserID"];
            DateTime vdate = DateTime.Now.Date;
            int business_division = 1;
            int vendor_id = 0;


            if (paymenttype == 1) { vendor_id = (int)Creditvendor_id; }
            if (paymenttype == 2) { vendor_id = (int)cashVendor_id; }



            // add inventory
            int inventory_type_id = (int)db.Inventory_Types.Where(a => a.Inventory_Type == "Purchase").Select(a => a.Inventory_type_id).FirstOrDefault();
            db.tblInventory_Transactions.Add(new Models.tblInventory_Transactions { Business_Division_Id = business_division, Total = 0, Tran_Date = vdate, Status = "Open", UserID = userid, Payment_Method = "N/A", Inventory_type_id = inventory_type_id, Tran_Nbr = "TRAN123", Vendor_ID = vendor_id });
            db.SaveChanges();
            int last_inventory_id = 0;
            last_inventory_id = db.tblInventory_Transactions.Select(a => a.Inventory_Tran_ID).Max();
            for (int x = 0; x <= item_id.Length - 1; x++)
            {
                int itemid = item_id[x];
                // Update Item Price / Cost
                float costx = unit_cost[x];
                float pricex = price[x];
                var temp = db.Items.Where(a => a.Item_Id == itemid).FirstOrDefault();
                if (temp != null)
                {
                    temp.Sale_Price = (decimal)pricex;
                }
                // Update Item Price / Cost
                float per_freight = 0;
                float per_tax = 0;
                float t_cost = 0;

                if (freight_rate == null) { freight_rate = 0; }
                if (tax_rate == null) { tax_rate = 0; }

                t_cost = unit_cost[x] * qty[x];
                per_freight = (float)freight_rate * t_cost;
                per_tax = (float)tax_rate * t_cost;

                float per_freight_qty = per_freight / qty[x];  // Devide the Freight per 1 item
                float per_tax_qty = per_tax / qty[x];  // Devide the Tax per 1 item
                db.tblInventory_Transaction_Details.Add(new Models.tblInventory_Transaction_Details { Inventory_Tran_ID = last_inventory_id, Item_Id = item_id[x], Qty_In = qty[x], Qty_Out = 0, Unit_Price = unit_cost[x], Sales_price = price[x], Batch = lot[x], Freight = per_freight_qty, Tax = per_tax_qty, Barcode = barcode[x] });
                db.SaveChanges();
                if (productdate != null && expiredate != null)
                {
                    int last_inventory_Detail_id = db.tblInventory_Transaction_Details.Select(a => a.Inventory_Tran_Detail_ID).Max();
                    db.tblBatches.Add(new Models.tblBatch { Batch = lot[x], Manufacture_Date = productdate[x], Expire_Date = expiredate[x], Inventory_Detail_id = last_inventory_Detail_id });
                }




                // MAKE AVARAGE COST

                int NumberOfTimesPurchased = db.tblInventory_Transaction_Details.Where(a => a.Item_Id == itemid && a.Qty_In > 0).Select(a => a.Item_Id).Count();
                float totalCostOfItem = db.tblInventory_Transaction_Details.Where(a => a.Item_Id == itemid && a.Qty_In > 0).Select(a => a.Unit_Price).Sum();
                decimal avg_cost = (decimal)(totalCostOfItem / NumberOfTimesPurchased);
                temp.Unit_Cost = avg_cost;
                db.SaveChanges();
                // MAKE AVARAGE COST

            }
            db.SaveChanges();
            float total = 0;
            total = (float)db.tblInventory_Transaction_Details.Where(a => a.Inventory_Tran_ID == last_inventory_id).Select(a => a.Qty_In * (a.Unit_Price + a.Freight + a.Tax)).Sum();


            int serialno = 0;
            int? sr = db.tblInvoices.Where(a => a.Invoice_id == 1).Select(a => a.Invoice_No).FirstOrDefault();
            if (sr == null || sr == 0)
            {
                serialno = int.Parse(db.tblSettings.Where(a => a.ID == 1).Select(a => a.Value).FirstOrDefault());
            }
            else
            {
                serialno = (int)sr + 1;
            }








            var p = db.tblInventory_Transactions.Where(f => f.Inventory_Tran_ID == last_inventory_id).FirstOrDefault();
            p.Total = total;
            p.Invoice_No = serialno;





            //add log
            helper.sitemethods.addlog("Add Data", "Add Manual Inventory  (" + item_id.Length + ") Items  ", divid, userid);








            // Add Journal
            Models.tblJournal j = new Models.tblJournal();

            j.Journal_Date = vdate;
            j.Tran_Nbr = tranNbr;
            j.TYear = DateTime.Now.Year;
            j.Period = DateTime.Now.Month;
            j.UserID = userid;
            j.Status = "Open";
            j.Tran_Type = "Inventory";
            j.Description = "Manual Inventory " + desc;
            db.tblJournals.Add(j);
            db.SaveChanges();
            int Journal_id = db.tblJournals.Where(a => a.UserID == userid).Select(a => a.Journal_id).Max();



            if (paymenttype == 2)
            {


                // cash inventory
                // inventory
                db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = total, Credit = 0, Account_Number = (int)cashinvacc });
                db.SaveChanges();
                // cashacc
                db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = total, Account_Number = (int)cashacc });
                db.SaveChanges();
                // Add cash flow 
                int Journal_Detail_id = db.tblJournal_Details.Where(a => a.Journal_id == Journal_id).Select(a => a.Journal_Detail_id).Max();
                db.tblCash_Flow.Add(new Models.tblCash_Flow {Debit = 0, Credit = total, Journal_Detail_id = Journal_Detail_id, Description = "Manual Inventory Payment", Tran_Nbr = "TRN", Tran_Type = "Cash Out" });
                db.SaveChanges();

                var vendorname = db.tblVendors.Where(a => a.Vendor_ID == vendor_id).Select(a => a.Vendor_Name).FirstOrDefault();
                // Voucher
                var x = db.tblReceipt_Voucher.Add(new Models.tblReceipt_Voucher { Receipt_Date = vdate, Tran_Nbr = "N/A",  Description = "Inventory Purchase Payment", Payee = vendorname,  UserID = userid, Amount = total, In_Word = "" });
                db.SaveChanges();
                int voucher_id = db.tblReceipt_Voucher.Where(a => a.UserID == userid).Select(a => a.Voucher_ID).Max();
                //UpdateVoucher_Number((int)voucher_id);
                return RedirectToAction("PaymentVoucher", "Accounting", new { id = voucher_id });
            }

            if (paymenttype == 1)
            {

                //Credit Inventory

                // inventory
                db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = total, Credit = 0, Account_Number = (int)creditinvacc });
                db.SaveChanges();
                // Payable
                db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = total, Account_Number = (int)creditpayableacc });
                db.SaveChanges();


                // Add vendor Transaction
                int? vendor_tran_id = null;

                try
                {
                    vendor_tran_id = db.tblVendor_Transaction.Where(a => a.Status == "Open" && a.VendorID == vendor_id).Select(a => a.Vendor_Tran_ID).Max();
                }
                catch (Exception e)
                {
                }

                if (vendor_tran_id == null)
                {
                    // Add Vendor Transaction
                    db.tblVendor_Transaction.Add(new Models.tblVendor_Transaction { VendorID = vendor_id, Tran_Date = DateTime.Now, UserID = userid, Status = "Open", Amount = total });
                    db.SaveChanges();
                    vendor_tran_id = db.tblVendor_Transaction.Where(a => a.UserID == userid).Select(a => a.Vendor_Tran_ID).Max();

                }
                int? vendor_tran_type_id = db.tblVendor_Tran_Type.Where(a => a.Tran_Type == "Vendor Invoice").Select(a => a.Vendor_Tran_Type_ID).FirstOrDefault();
                db.tblVendor_Transaction_Detail.Add(new Models.tblVendor_Transaction_Detail { Vendor_Tran_ID = (int)vendor_tran_id, Vendor_Tran_Type_ID = vendor_tran_type_id, Debit = 0, Credit = total, Description = "Manual Inventory Purchase", Tran_Nbr = tranNbr, UserID = userid, Trans_Date = vdate });
                db.SaveChanges();
                db.Sp_Update_Vendor_Trans_Total(vendor_tran_id);

            }


            if (paymenttype == 3)
            {

                // Initial Inventory

                // inventory
                db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = total, Credit = 0, Account_Number = (int)assetinvacc });
                db.SaveChanges();
                // asset
                db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = total, Account_Number = (int)asset });
                db.SaveChanges();
            }


            return RedirectToAction("success", "message", new { msg = "Inventory Added  Successfully !" });
        }











        public ActionResult PostPurchase()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PostPurchaseOrder(int purchase_id)
        {

            ViewBag.id = purchase_id;
            return View();
        }






        public ActionResult PostPurchaseNow(int purchase_id, int ptype, int? cashacc, int? invacc, int? payableacc, int? vendorid, string desc)
        {
            int divid = 1;
            int userid = (int)Session["UserID"];

            //add log
            SameSoftWeb.helper.sitemethods.addlog("Add Data", "Posting Purchase Order # " + purchase_id, divid, userid);




            float amount = 0;
            string tran_nbr = "TRAN123";
            string paymethod = "";
            var purchaseData = db.tblInventory_Transactions.Where(a => a.Inventory_Tran_ID == purchase_id).FirstOrDefault();
            if (purchaseData != null)
            {
                amount = purchaseData.Total;
                DateTime vdate = DateTime.Now.Date;

                if (ptype == 1) // invoice
                {
                    paymethod = "Invoice";
                    // Add Journal
                    Models.tblJournal j = new Models.tblJournal();
                    j.Journal_Date = vdate;
                    j.Tran_Nbr = tran_nbr;
                    j.TYear = DateTime.Now.Year;
                    j.Period = DateTime.Now.Month;
                    j.UserID = userid;
                    j.Status = "Open";
                    j.Tran_Type = "Purchase";
                    j.Description = "Posted Credit Inventory - " + desc;
                    db.tblJournals.Add(j);
                    db.SaveChanges();
                    int Journal_id = db.tblJournals.Where(a => a.UserID == userid).Select(a => a.Journal_id).Max();
                    db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = (int)payableacc });
                    db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = (int)invacc });
                    db.SaveChanges();

                    int? vendor_tran_id = null;

                    try
                    {
                        vendor_tran_id = db.tblVendor_Transaction.Where(a => a.Status == "Open" && a.VendorID == (int)vendorid).Select(a => a.Vendor_Tran_ID).Max();
                    }
                    catch (Exception ex)
                    {

                    }
                    if (vendor_tran_id == null)
                    {
                        // Add Vendor Transaction
                        db.tblVendor_Transaction.Add(new Models.tblVendor_Transaction { VendorID = (int)vendorid, Tran_Date = DateTime.Now, UserID = userid, Status = "Open", Amount = amount });
                        db.SaveChanges();
                        vendor_tran_id = db.tblVendor_Transaction.Where(a => a.UserID == userid).Select(a => a.Vendor_Tran_ID).Max();

                    }

                    int? vendor_tran_type_id = db.tblVendor_Tran_Type.Where(a => a.Tran_Type == "Vendor Invoice").Select(a => a.Vendor_Tran_Type_ID).FirstOrDefault();
                    db.tblVendor_Transaction_Detail.Add(new Models.tblVendor_Transaction_Detail { Vendor_Tran_ID = (int)vendor_tran_id, Vendor_Tran_Type_ID = vendor_tran_type_id, Debit = 0, Credit = amount, Description = desc, Tran_Nbr = tran_nbr, UserID = userid, Inventory_Tran_ID = purchase_id, Trans_Date = vdate });
                    db.SaveChanges();

                    db.Sp_Update_Vendor_Trans_Total(vendor_tran_id);


                }


                if (ptype == 2)
                {

                    paymethod = "Cash";



                    Models.tblJournal j = new Models.tblJournal();
                    j.Journal_Date = DateTime.Now;
                    j.Tran_Nbr = "TR123";
                    j.TYear = DateTime.Now.Year;
                    j.Period = DateTime.Now.Month;
                    j.UserID = userid;
                    j.Status = "Open";
                    j.Tran_Type = "Purchase";
                    j.Description = "Posted Cash Inventory - " + desc;
                    db.tblJournals.Add(j);
                    db.SaveChanges();
                    int Journal_id = db.tblJournals.Where(a => a.UserID == userid).Select(a => a.Journal_id).Max();
                    db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = (int)cashacc });
                    db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = (int)invacc });
                    db.SaveChanges();
                }

                purchaseData.Status = "Posted";
                purchaseData.Payment_Method = paymethod;
                db.SaveChanges();
            }


            return RedirectToAction("View_Msg", "Home", new { msg = "Transaction  Completed Successfully !" });


        }
 
        public ActionResult Addexpense(int? id)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(6);


            ViewBag.id = id;

            return View();
        }

        [HttpPost]
        public ActionResult Addexpense(int type, DateTime trandate, string note, float amount)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(6);

            int userid = (int)Session["UserID"];

            string strTran_Nbr = DateTime.UtcNow.AddHours(3).ToString().GetHashCode().ToString("x");

            if (type == 8)
            {
                db.tblExpenses.Add(new Models.tblExpense { Expense_Type_ID = type, Dr = amount, Cr = 0,Balance=0, Expense_Date = trandate, UserID = userid, Period = trandate.Month, Year = trandate.Year, Note = note, Status = "Posted", Tran_Nbr = strTran_Nbr });
                db.SaveChanges();
            }
            else
            {
                db.tblExpenses.Add(new Models.tblExpense { Expense_Type_ID = type, Dr = 0, Cr = amount, Balance = 0, Expense_Date = trandate, UserID = userid, Period = trandate.Month, Year = trandate.Year, Note = note, Status = "Open", Tran_Nbr = strTran_Nbr });
                db.SaveChanges();
                string typename = db.tblExpenseTypes.Where(a => a.Expense_Type_ID == type).Select(a => a.Expense_Type).FirstOrDefault();

                helper.mnc.addlog("Expense", "Added New Expense of type  " + typename + " Amonut= " + amount, userid);
            }
       

        

            ViewBag.msg = "Expense Added Successfully";
         
            return RedirectToAction("ExpensePayment", "Accounting");

        }



        public ActionResult ExpensePayment(int? id)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(7);

            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public ActionResult ExpensePayment(int paymenttype, int? acc, int? vendor, string payee, int id, DateTime? trandate)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(7);

            DateTime vdate;
            int userid = (int)Session["UserID"];
            if (trandate != null)
            {
                vdate = (DateTime)trandate;

            }
            else { vdate = helper.mnc.getdate(); }

            var expense = db.tblExpenses.Where(a => a.Expense_ID == id).FirstOrDefault();

            var type = db.tblExpenseTypes.Where(a => a.Expense_Type_ID == expense.Expense_Type_ID).FirstOrDefault();



            db.Sp_Add_Journal(vdate, vdate.Month, vdate.Year, expense.Tran_Nbr, type.Expense_Type + " Expense #" + expense.Expense_ID + " Payment ", userid, "Open", "Expense Payment", "CP");
            int Journal_id = (int)db.SP_Get_Max_Journal_id_by_User(userid).FirstOrDefault();


            // add expense
            db.Sp_Add_Journal_Details(Journal_id, type.Account_Number, expense.Cr, 0);
            expense.Status = "Posted";
            db.SaveChanges();
            string pmetod = paymenttype == 1 ? "Cash" : "Credit";

            if (paymenttype == 1)
            {
                expense.Payment_Method = "Cash";
                db.SaveChanges();
                // cashh acc
                db.Sp_Add_Journal_Details(Journal_id, acc, 0, expense.Cr);
                int Jounal_Detail_id = (int)db.Sp_Get_Max_Journal_Detail_by_UserID(userid).FirstOrDefault();
                db.AddCashFlow("Cash Paid For", "" + type.Expense_Type + "Expense #" + expense.Expense_ID, Jounal_Detail_id, expense.Tran_Nbr, 0, expense.Cr);

                string strWord = SameSoftWeb.helper.mnc.ToWords(expense.Cr.ToString());

                string Note = db.tblExpenses.Where(a => a.Expense_ID == id).Select(a => a.Note).FirstOrDefault();

                db.Add_Payment_Voucher(vdate, expense.Cr, payee, strWord, userid, 1, Note, expense.Tran_Nbr);
                int intSelectValue = (int)db.Get_Max_VoucherID().FirstOrDefault();
                return RedirectToAction("PaymentVoucher", "Accounting", new { id = intSelectValue });


            }
            else
            {
                expense.Payment_Method = "Credit";
                db.SaveChanges();
                // Payable acc
                db.Sp_Add_Journal_Details(Journal_id, 2110, 0, expense.Cr);
                // add vendor Transaction
                int? intVendor_TranID = db.Get_Vendor_TransactionID_By_Status_VendorID("Open", vendor).FirstOrDefault();

                if (intVendor_TranID == null)
                {
                    db.Add_Vendor_Transaction(vdate, expense.Cr, "Open", userid, vendor);
                    intVendor_TranID = (int)db.Get_Vendor_TransactionID_By_Status_VendorID("Open", vendor).Max();

                }

                int intVendorTranTypeID = (int)db.Get_Vendor_Tran_Type_By_Tran_Type("Ticket Refund").FirstOrDefault();
                db.Add_Vendor_Transaction_Detail(intVendor_TranID, 0, expense.Cr, type.Expense_Type + " Expense #" + expense.Expense_ID, userid, vdate, intVendorTranTypeID, expense.Tran_Nbr, "Open");
                float intTotal_Vendor_Balance = (float)db.Get_Total_Vendor_Tranaction_Detial_by_Vendor_TranID(intVendor_TranID).FirstOrDefault();
                db.Update_Vendor_Transaction_Amount_By_Vendor_transid(intTotal_Vendor_Balance, intVendor_TranID);


                helper.mnc.addlog("Expense", "Expense #   " + id + " Paid by " + pmetod, userid);

                ViewBag.msg = "Expense Payment Transaction completed Successfully";
                return View("ExpensePayment");
            }





        }


        public ActionResult DeleteExpense(int id)
        {
        //    UsersController u = new UsersController();
        //    u.checkaccess(6);

            var exp = db.tblExpenses.Where(A => A.Expense_ID == id).FirstOrDefault();
            if (exp != null)
            {


                db.tblExpenses.Remove(exp);

                db.SaveChanges();
                ViewBag.msg = "Expense Deleted Successfully";
                helper.mnc.addlog("Expense", "Expense #   " + id + " Deleted amount " + exp.Cr, (int)Session["UserID"]);
                return View("ExpensePayment");
            }
            return Content("Invalid ID");
        }


        public ActionResult DeletePostedExpense(DateTime? from, DateTime? to)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(9);

            ViewBag.from = from;
            ViewBag.to = to;
            return View();
        }
        [HttpPost]
        public ActionResult DeletePostedExpense(int id)
        {
        //    UsersController u = new UsersController();
        //    u.checkaccess(9);

            var exp = db.tblExpenses.Where(A => A.Expense_ID == id && A.Status == "Posted").FirstOrDefault();
            if (exp != null)
            {



                string tran = exp.Tran_Nbr;
                db.tblExpenses.Remove(exp);

                // Remove Journals
                var jrn = db.tblJournals.Where(a => a.Tran_Nbr == tran).FirstOrDefault();
                if (jrn != null)
                {
                    int jiid = jrn.Journal_id;

                    var details = db.tblJournal_Details.Where(a => a.Journal_id == jiid).ToList();
                    if (details != null)
                    {
                        db.tblJournal_Details.RemoveRange(details);
                    }
                    db.tblJournals.Remove(jrn);

                    db.SaveChanges();

                }

                //Remove Payment voucher
                var pv = db.tblPayment_Voucher.Where(a => a.Tran_Nbr == tran).FirstOrDefault();
                if (pv != null)
                {
                    db.tblPayment_Voucher.Remove(pv);
                    db.SaveChanges();
                }



                //Remove Vendor Trabs
                var vend = db.tblVendor_Transaction_Detail.Where(a => a.Tran_Nbr == tran).FirstOrDefault();
                if (vend != null)
                {
                    int intVendor_TranID = vend.Vendor_Tran_ID;
                    db.tblVendor_Transaction_Detail.Remove(vend);
                    db.SaveChanges();

                    double? intTotal_Vendor_Balance = db.Get_Total_Vendor_Tranaction_Detial_by_Vendor_TranID(intVendor_TranID).FirstOrDefault();
                    intTotal_Vendor_Balance = intTotal_Vendor_Balance == null ? 0 : intTotal_Vendor_Balance;

                    db.Update_Vendor_Transaction_Amount_By_Vendor_transid((float)intTotal_Vendor_Balance, intVendor_TranID);

                }


                //Remove Cash Flow
                var cashflow = db.tblCash_Flow.Where(a => a.Tran_Nbr == tran).FirstOrDefault();
                if (cashflow != null)
                {
                    db.tblCash_Flow.Remove(cashflow);
                    db.SaveChanges();
                }

                db.SaveChanges();

                helper.mnc.addlog("Expense", "Posted Expense #   " + id + " Deleted and all associated Transactions ", (int)Session["UserID"]);
                ViewBag.msg = "Expense and all Associated transactions Deleted Successfully";
                return View("DeletePostedExpense");
            }
            return Content("Invalid ID");
        }






        public ActionResult ExpenseTypes(string msg)
        {

            //UsersController u = new UsersController();
            //u.checkaccess(8);

            ViewBag.msg = msg;
            return View();
        }
        public ActionResult AddExpenseType(int acc, string type)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(8);

            var exp = db.tblExpenseTypes.Where(a => a.Expense_Type == type).FirstOrDefault();
            if (exp != null)
            {
                ViewBag.msg = "Expense Type " + type + " Already Exist";
                return View("ExpenseTypes");
            }


            db.tblExpenseTypes.Add(new Models.tblExpenseType { Expense_Type = type, Account_Number = acc });
            db.SaveChanges();

            helper.mnc.addlog("Expense", "New  Expense Type   " + type + " Created ", (int)Session["UserID"]);
            ViewBag.msg = "Expense Type " + type + " Created";
            return View("ExpenseTypes");

        }




        public ActionResult UpdateExpenseType(int id, string type)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(8);

            var exp = db.tblExpenseTypes.Where(a => a.Expense_Type == type).FirstOrDefault();
            if (exp != null)
            {
                ViewBag.msg = "Expense Type " + type + " Already Exist";
                return View("ExpenseTypes");
            }

            var ex = db.tblExpenseTypes.Where(a => a.Expense_Type_ID == id).FirstOrDefault();
            ex.Expense_Type = type;
            db.SaveChanges();


            helper.mnc.addlog("Expense", "Expense Type ID " + id + " Updated to  " + type, (int)Session["UserID"]);

            return RedirectToAction("ExpenseTypes", "Accounting", new { msg = "Expense Type Updated Successfully" });

        }

        public ActionResult DeleteExpenseType(int id)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(8);

            var ex = db.tblExpenseTypes.Where(a => a.Expense_Type_ID == id).FirstOrDefault();
            if (ex == null)
            {
                return View("ExpenseTypes");
            }

            var expense = db.tblExpenses.Where(a => a.Expense_Type_ID == id).FirstOrDefault();
            if (expense != null)
            {
                ViewBag.msg = "Expense Type  Could not be deleted ; There are Transactions associated with this Expense Type; Please Update instead.";
                return View("ExpenseTypes");
            }


            db.tblExpenseTypes.Remove(ex);
            db.SaveChanges();

            helper.mnc.addlog("Expense", "Expense Type    " + ex.Expense_Type + " Deleted  ", (int)Session["UserID"]);

            ViewBag.msg = "Expense Type Deleted Successfully";
            return View("ExpenseTypes");
        }

        


        public ActionResult CreateCustomerInvoice(int? id , string name)
        {
            ViewBag.id = id;
            ViewBag.name = name;
            //UsersController u = new UsersController();
            //u.checkaccess(78);

            return View();
        }



        public ActionResult AddCustomerInvoice(int customer,int invoice_number, float amount, float qty, float unit_price, string desc, int income, DateTime trandate)
        {

            //UsersController u = new UsersController();
            //u.checkaccess(78);
            string inwords = helper.mnc.ToWords(amount.ToString());
            int araccount = 1150;
            int userid = (int)Session["UserID"];
            DateTime vdate = trandate;
          
            // Add Journal
            Models.tblJournal j = new Models.tblJournal();
            string tranNbr = DateTime.UtcNow.AddHours(3).ToString().GetHashCode().ToString("x");
            j.Journal_Date = vdate;
            j.Tran_Nbr = tranNbr;
            j.TYear = DateTime.UtcNow.AddHours(3).Year;
            j.Period = DateTime.UtcNow.AddHours(3).Month;
            j.UserID = userid;
            j.Status = "Open";
            j.Tran_Type = "Customer Invoice";
            j.Description = desc;
            db.tblJournals.Add(j);
            db.SaveChanges();
            int Journal_id = db.tblJournals.Where(a => a.UserID == userid).Select(a => a.Journal_id).Max();
            // AR
            db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = araccount });
            db.SaveChanges();
            // Income
            db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = income });
            db.SaveChanges();
            double balance = 0;
            var Cust_balance = db.sp_Get_Customer_with_Balance2(customer).FirstOrDefault();
            if(Cust_balance != null)
            {
                balance = Cust_balance.Total;
            }
          
            // Add Customer Transaction

            int? cust_tran_id = null;
            try
            {
                cust_tran_id = db.tblCustomer_Transaction.Where(a => a.CustomerID == customer && a.Status == "Open").Select(a => a.Cust_Tran_ID).Max();
            }
            catch (Exception ex) { }


            if (cust_tran_id == null)
            {
                db.tblCustomer_Transaction.Add(new Models.tblCustomer_Transaction { Amount = amount, CustomerID = customer, Status = "Open", Tran_Date = vdate, UserID = userid });
                db.SaveChanges();
                cust_tran_id = db.tblCustomer_Transaction.Where(a => a.UserID == userid).Select(a => a.Cust_Tran_ID).Max();
            }

            int Cust_Tran_Type = 0;
            Cust_Tran_Type = db.tblCustomer_Tran_Type.Where(a => a.Tran_Type == "Customer Invoice").Select(a => a.Cust_Tran_Type_ID).FirstOrDefault();
            db.tblCustomer_Transaction_Detail.Add(new Models.tblCustomer_Transaction_Detail { Debit = amount, Credit = 0, Cust_Tran_Type_ID = Cust_Tran_Type, Status = "Open", UserID = userid, Description = desc, Tran_Nbr = tranNbr, Trans_Date = vdate, Cust_Tran_ID = (int)cust_tran_id, Qty = qty, Unit_price = unit_price ,Invoice_No=invoice_number, Route=inwords, Balance=(float)balance});
            db.SaveChanges();
            float intTotal_Vendor_Balance = (float)db.Get_Total_Customer_Transaction_Detail_by_TranID(cust_tran_id).FirstOrDefault();
            db.Update_Customer_Tranaction_Amount_By_Cust_tran_id(intTotal_Vendor_Balance, cust_tran_id);

            var customername = db.tblCustomers.Where(a => a.CustomerID == customer).Select(a => a.Customer_Name).FirstOrDefault();

            var x = db.tblInvoices.FirstOrDefault();
            if (x != null)
            {
                x.Invoice_No = invoice_number;

                db.SaveChanges();
            }

            //add log
            helper.mnc.addlog("Accounting", "Add Customer Invoice For Customer " + customername + " Amount  " + amount, userid);

            return RedirectToAction("PrintInvoice", "Accounting", new { id = invoice_number, msg = "Customer Invoice Created Successfully !" });
            
        }

        public ActionResult PrintInvoice(int id)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(14);

            ViewBag.id = id;
            return View();
        }






        public ActionResult VendorInvoice()
        {
            return View();
        }
        public ActionResult AddVendorInvoice(int vendor, float amount, string desc, int payableaccount, int expenseaccount)
        {

            string tranNbr = DateTime.Now.ToString().GetHashCode().ToString("x");
        
            int userid = (int)Session["userid"];
            DateTime vdate = DateTime.Now.Date;




            // Add Journal
            tblJournal j = new tblJournal();
            j.Journal_Date = vdate;
            j.Tran_Nbr = tranNbr;
            j.TYear = DateTime.Now.Year;
            j.Period = DateTime.Now.Month;
            j.UserID = userid;
            j.Status = "Open";
            j.Tran_Type = "Vendor Invoice";
            j.Description = desc;
            db.tblJournals.Add(j);
            db.SaveChanges();
            int Journal_id = db.tblJournals.Where(a => a.UserID == userid).Select(a => a.Journal_id).Max();
            // Expense
            db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = expenseaccount });
            // Payable
            db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = payableaccount });
            db.SaveChanges();
            db.SaveChanges();

            int? vendor_tran_id = null;

            try
            {
                vendor_tran_id = db.tblVendor_Transaction.Where(a => a.Status == "Open" && a.VendorID == vendor).Select(a => a.Vendor_Tran_ID).Max();
            }
            catch (Exception ex)
            {

            }
            if (vendor_tran_id == null)
            {
                // Add Vendor Transaction
                db.tblVendor_Transaction.Add(new tblVendor_Transaction { VendorID = vendor, Tran_Date = DateTime.Now, UserID = userid, Status = "Open", Amount = amount });
                db.SaveChanges();
                vendor_tran_id = db.tblVendor_Transaction.Where(a => a.UserID == userid).Select(a => a.Vendor_Tran_ID).Max();

            }


            int? vendor_tran_type_id = db.tblVendor_Tran_Type.Where(a => a.Tran_Type == "Vendor Invoice").Select(a => a.Vendor_Tran_Type_ID).FirstOrDefault();
            db.tblVendor_Transaction_Detail.Add(new tblVendor_Transaction_Detail { Vendor_Tran_ID = (int)vendor_tran_id, Vendor_Tran_Type_ID = vendor_tran_type_id, Debit = 0, Credit = amount, Description = desc, Tran_Nbr = tranNbr, UserID = userid, Trans_Date = vdate });
            db.SaveChanges();
            int trans_detail_id = db.tblVendor_Transaction_Detail.Where(a => a.Vendor_Tran_ID == vendor_tran_id).Select(a => a.Vendor_Transaction_Detail_id).Max();
            db.Sp_Update_Vendor_Trans_Total(vendor_tran_id);

            var vendorname = db.tblVendors.Where(a => a.Vendor_ID == vendor).Select(a => a.Vendor_Name).FirstOrDefault();
            //add log
            helper.mnc.addlog("Add Data", "Add Vendor Invoice For Vendor " + vendorname + " Amount  " + amount, userid);


            return RedirectToAction("VendorBalance", "Accounting", new { msg = "Vendor Invoice Created Successfully !" });
            /// 
            //return RedirectToAction("InstallmentPlan", "Vendors", new { vendor=vendor, trans_detail_id= trans_detail_id,total=amount,id=0 });


        }



        public ActionResult CustomerInvoice()
        {

            return View();
        }


        public ActionResult AddCustomerInvoice(int customer, float amount, string desc, int araccount, int income)
        {

            string tranNbr = DateTime.Now.ToString().GetHashCode().ToString("x");

            int userid = (int)Session["userid"];
            DateTime vdate = DateTime.Now.Date;
            // Add Journal
            tblJournal j = new tblJournal();
            j.Journal_Date = vdate;
            j.Tran_Nbr = tranNbr;
            j.TYear = DateTime.Now.Year;
            j.Period = DateTime.Now.Month;
            j.UserID = userid;
            j.Status = "Open";
            j.Tran_Type = "Customer Invoice";
            j.Description = desc;
            db.tblJournals.Add(j);
            db.SaveChanges();
            int Journal_id = db.tblJournals.Where(a => a.UserID == userid).Select(a => a.Journal_id).Max();
            // AR
            db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = araccount });
            db.SaveChanges();
            // Income
            db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = income });
            db.SaveChanges();






            // Add Customer Transaction

            int? cust_tran_id = null;
            try
            {
                cust_tran_id = db.tblCustomer_Transaction.Where(a => a.CustomerID == customer && a.Status == "Open").Select(a => a.Cust_Tran_ID).Max();
            }
            catch (Exception ex) { }


            if (cust_tran_id == null)
            {
                db.tblCustomer_Transaction.Add(new tblCustomer_Transaction { Amount = amount, CustomerID = customer, Status = "Open", Tran_Date = vdate, UserID = userid });
                db.SaveChanges();
                cust_tran_id = db.tblCustomer_Transaction.Where(a => a.UserID == userid).Select(a => a.Cust_Tran_ID).Max();
            }

            int Cust_Tran_Type = 0;
            Cust_Tran_Type = db.tblCustomer_Tran_Type.Where(a => a.Tran_Type == "Customer Invoice").Select(a => a.Cust_Tran_Type_ID).FirstOrDefault();
            db.tblCustomer_Transaction_Detail.Add(new tblCustomer_Transaction_Detail { Debit = amount, Credit = 0, Cust_Tran_Type_ID = Cust_Tran_Type, Status = "Open", UserID = userid, Description = desc, Tran_Nbr = tranNbr, Trans_Date = vdate, Cust_Tran_ID = (int)cust_tran_id });
            db.SaveChanges();
            db.Sp_Update_Customer_Trans_Total(cust_tran_id);


            var customername = db.tblCustomers.Where(a => a.CustomerID == customer).Select(a => a.Customer_Name).FirstOrDefault();


            //add log
            helper.mnc.addlog("Add Data", "Add Customer Invoice For Customer " + customername + " Amount  " + amount, userid);


            return RedirectToAction("success", "message", new { msg = "Customer Invoice Created Successfully !" });


        }








        public ActionResult VendorPrepaid()
        {
            return View();
        }


        public ActionResult AddVendorPrepaid(int vendor, float amount, string desc, int payableaccount, int cashaccount)
        {

            string tranNbr = DateTime.Now.ToString().GetHashCode().ToString("x");

            int userid = (int)Session["userid"];
            DateTime vdate = DateTime.Now.Date;
            string vendorname = db.tblVendors.Where(a => a.Vendor_ID == vendor).Select(a => a.Vendor_Name).FirstOrDefault();
            // Add Journal
            tblJournal j = new tblJournal();
            j.Journal_Date = vdate;
            j.Tran_Nbr = tranNbr;
            j.TYear = DateTime.Now.Year;
            j.Period = DateTime.Now.Month;
            j.UserID = userid;
            j.Status = "Open";
            j.Tran_Type = "Vendor Prepaid";
            j.Description = desc;
            db.tblJournals.Add(j);
            db.SaveChanges();
            int Journal_id = db.tblJournals.Where(a => a.UserID == userid).Select(a => a.Journal_id).Max();
            // Payable
            db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = payableaccount });
            db.SaveChanges();
            // cashacc
            db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = cashaccount });
            db.SaveChanges();

            // Add cash flow 
            int Journal_Detail_id = db.tblJournal_Details.Where(a => a.Journal_id == Journal_id).Select(a => a.Journal_Detail_id).Max();
            db.tblCash_Flow.Add(new tblCash_Flow { Debit = 0, Credit = amount, Journal_Detail_id = Journal_Detail_id, Description = "Prepaid vendor :" + vendorname + "; " + desc, Tran_Nbr = tranNbr, Tran_Type = "Cash Out" });
            db.SaveChanges();

            int? vendor_tran_id = null;

            try
            {
                vendor_tran_id = db.tblVendor_Transaction.Where(a => a.Status == "Open" && a.VendorID == vendor).Select(a => a.Vendor_Tran_ID).Max();
            }
            catch (Exception ex)
            {

            }
            if (vendor_tran_id == null)
            {
                // Add Vendor Transaction
                db.tblVendor_Transaction.Add(new tblVendor_Transaction { VendorID = vendor, Tran_Date = DateTime.Now, UserID = userid, Status = "Open", Amount = amount });
                db.SaveChanges();
                vendor_tran_id = db.tblVendor_Transaction.Where(a => a.UserID == userid).Select(a => a.Vendor_Tran_ID).Max();

            }

            int? vendor_tran_type_id = db.tblVendor_Tran_Type.Where(a => a.Tran_Type == "Vendor Prepiad").Select(a => a.Vendor_Tran_Type_ID).FirstOrDefault();

            db.tblVendor_Transaction_Detail.Add(new tblVendor_Transaction_Detail { Vendor_Tran_ID = (int)vendor_tran_id, Vendor_Tran_Type_ID = vendor_tran_type_id, Debit = amount, Credit = 0, Description = desc, Tran_Nbr = "--", UserID = userid, Inventory_Tran_ID = 0, Trans_Date = vdate });
            db.SaveChanges();
            db.Sp_Update_Vendor_Trans_Total(vendor_tran_id);

            var x = db.tblPayment_Voucher.Add(new tblPayment_Voucher { Payment_Date = vdate, Tran_Nbr = tranNbr, Description = "Vendor Prepaid", Payee = vendorname, UserID = userid, Amount = amount, In_Word = "" });
            db.SaveChanges();

            int voucher_id = db.tblPayment_Voucher.Where(a => a.UserID == userid).Select(a => a.Voucher_ID).Max();
            //UpdateVoucher_Number((int)voucher_id);
            //add log
            helper.mnc.addlog("Add Data", "Add Vendor Prepaid Invoice For Vendor " + vendorname + " Amount  " + amount, userid);


            return RedirectToAction("PaymentVoucher", "Accounting", new { id = voucher_id });


        }

        public ActionResult VendorTransaction(int? Vendor, DateTime? from, DateTime? to)
        {
            ViewBag.from = from;
            ViewBag.to = to;
            ViewBag.id = Vendor;

            return View();
        }

        public ActionResult VendorStatement(int? id)
        {
            ViewBag.id = id;


            return View();
        }




        public ActionResult VendorPayment(int vendorid, int trantype, float amount, string desc, int? cashacc, int? discountacc, int? payableacc, DateTime trandate, string Payment_Method, string check_number, string cashier_name)
        {
            string tranNbr = DateTime.Now.ToString().GetHashCode().ToString("x");
       
            int userid = (int)Session["userid"];
            DateTime vdate = trandate;// DateTime.Now.Date;
            string vendorname = "";
            string inwords = helper.mnc.ToWords(amount.ToString());
            int? voucher_id;
            int? vendor_tran_id = db.tblVendor_Transaction.Where(a => a.Status == "Open" && a.VendorID == vendorid).Select(a => a.Vendor_Tran_ID).FirstOrDefault();

           

            if (vendor_tran_id == null)
            {
                // Add Vendor Transaction
                db.tblVendor_Transaction.Add(new tblVendor_Transaction { VendorID = (int)vendorid, Tran_Date = DateTime.Now, UserID = userid, Status = "Open", Amount = amount });
                db.SaveChanges();
                vendor_tran_id = db.tblVendor_Transaction.Where(a => a.UserID == userid).Select(a => a.Vendor_Tran_ID).Max();

            }
            vendorname = db.tblVendors.Where(a => a.Vendor_ID == vendorid).Select(a => a.Vendor_Name).FirstOrDefault();

            //add log
            helper.mnc.addlog("Add Data", "Vendor Payment for Vendor" + vendorname + " Amount:" + amount, userid);



            if (trantype == 2)
            {
                db.tblVendor_Transaction_Detail.Add(new tblVendor_Transaction_Detail { Debit = amount, Credit = 0, Vendor_Tran_Type_ID = trantype, UserID = userid, Description = "Vendor Payment - " + desc, Tran_Nbr = tranNbr, Trans_Date = vdate, Vendor_Tran_ID = (int)vendor_tran_id });
            }
            if (trantype == 3)
            {
                db.tblVendor_Transaction_Detail.Add(new tblVendor_Transaction_Detail { Debit = amount, Credit = 0, Vendor_Tran_Type_ID = trantype, UserID = userid, Description = "Vednor Discount -" + desc, Tran_Nbr = tranNbr, Trans_Date = vdate, Vendor_Tran_ID = (int)vendor_tran_id });
            }

            if (trantype == 4)
            {
                db.tblVendor_Transaction_Detail.Add(new tblVendor_Transaction_Detail { Debit = 0, Credit = amount, Vendor_Tran_Type_ID = trantype, UserID = userid, Description = "Vednor Refund -" + desc, Tran_Nbr = tranNbr, Trans_Date = vdate, Vendor_Tran_ID = (int)vendor_tran_id });
            }


            db.SaveChanges();
            // update total
            db.Sp_Update_Vendor_Trans_Total(vendor_tran_id);


            // Add Journal
            tblJournal j = new tblJournal();
            j.Journal_Date = vdate;
            j.Tran_Nbr = tranNbr;
            j.TYear = DateTime.Now.Year;
            j.Period = DateTime.Now.Month;
            j.UserID = userid;
            j.Status = "Open";
            j.Tran_Type = "Vendors";
            j.Description = "Veddor Transaction " + vendorname + " " + desc;
            db.tblJournals.Add(j);
            db.SaveChanges();
            int Journal_id = db.tblJournals.Where(a => a.UserID == userid).Select(a => a.Journal_id).Max();

            if (trantype == 2)
            {
                // Acc Payable
                db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = (int)payableacc });
                // Add cash flow 

                // Cash
                db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = (int)cashacc });
                db.SaveChanges();

                int Journal_Detail_id = db.tblJournal_Details.Where(a => a.Journal_id == Journal_id).Select(a => a.Journal_Detail_id).Max();
                db.tblCash_Flow.Add(new tblCash_Flow { Debit = 0, Credit = amount, Journal_Detail_id = Journal_Detail_id, Description = "Cash Paid for VendorID " + vendorid, Tran_Nbr = tranNbr, Tran_Type = "Cash Out" });
                db.SaveChanges();


            }

            if (trantype == 3)
            {
                // Acc Payable
                db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = (int)payableacc });
                // Purchase Discount 
                db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = (int)discountacc });
                db.SaveChanges();

            }



            if (trantype == 4)
            {
                // Cash
                db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = amount, Credit = 0, Account_Number = (int)cashacc });
                db.SaveChanges();
                // Add cash flow 
                int Journal_Detail_id = db.tblJournal_Details.Where(a => a.Journal_id == Journal_id).Select(a => a.Journal_Detail_id).Max();
                db.tblCash_Flow.Add(new tblCash_Flow { Debit = amount, Credit = 0, Journal_Detail_id = Journal_Detail_id, Description = "Cash Received from VendorID " + vendorid, Tran_Nbr = tranNbr, Tran_Type = "Cash Out" });
                // Acc Payable
                db.tblJournal_Details.Add(new tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = amount, Account_Number = (int)payableacc });
                db.SaveChanges();


            }


            double? balance = 0;
            try
            {
                DateTime d1 = DateTime.Now;
                DateTime d2 = DateTime.Now;
                balance = db.Sp_Get_Vendor_with_Balance().Where(a => a.Vendor_ID == vendorid).Select(a => a.BALANCE).FirstOrDefault();
            }
            catch (Exception e) { }

            if (balance == null)
            {
                balance = 0;
            }

            if (vdate == DateTime.UtcNow.AddHours(3).Date)
            {
                vdate = DateTime.UtcNow.AddHours(3);
            }

            // add vouchers
            if (trantype == 2)
            {
                var x = db.tblPayment_Voucher.Add(new tblPayment_Voucher { Payment_Date = vdate, Tran_Nbr = tranNbr, Description = "Vendor Payment", Payee = vendorname, UserID = userid, Amount = amount, In_Word = inwords });
                db.SaveChanges();

                voucher_id = db.tblPayment_Voucher.Where(a => a.UserID == userid).Select(a => a.Voucher_ID).Max();
             //UpdateVoucher_Number((int)voucher_id);
                return RedirectToAction("PaymentVoucher", "Accounting", new { id = voucher_id });
            }

            if (trantype == 4)
            {
                var x = db.tblPayment_Voucher.Add(new tblPayment_Voucher { Payment_Date = vdate, Tran_Nbr = tranNbr, Description = "Vendor Refund", Payee = vendorname, UserID = userid, Amount = amount, In_Word = inwords });
                db.SaveChanges();

                voucher_id = db.tblPayment_Voucher.Where(a => a.UserID == userid).Select(a => a.Voucher_ID).Max();
                //UpdateVoucher_Number((int)voucher_id);
                return RedirectToAction("ReceiptVoucher", "Accounting", new { id = voucher_id });
            }





            return RedirectToAction("VendorTrans", "Accounting", new { id = vendorid, msg = "Vendor Transaction Recorded Successfully" });


        }



        public ActionResult VendorBalance(DateTime? from, DateTime? to, int? dividlist)
        {
            ViewBag.from = from;
            ViewBag.to = to;
            ViewBag.divid = dividlist;

            return View();
        }
        public ActionResult VendorTrans(int? id)
        {
            ViewBag.id = id;

            return View();
        }






    }
}