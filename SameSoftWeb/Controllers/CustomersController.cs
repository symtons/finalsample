using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SameSoftWeb.Controllers
{
    public class CustomersController : Controller
    {
        // GET: Customers
        SameSoftWeb.Models.SameSoftwareWebEntities db = new SameSoftWeb.Models.SameSoftwareWebEntities();
        public ActionResult CustomerPost(int? month, int? year)
        {

            ViewBag.year = year;
            ViewBag.month = month;

            return View();
        }

        


        public ActionResult PostRateNow(int[] Cust_ID, float[] amount, DateTime trandate)
        {
            int i = 0;

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
            int UserID = (int)Session["UserID"];
      
            for (int x = 0; x <= Cust_ID.Length - 1; x++)
            {
            
                int ID = Cust_ID[x];
                float Mantainance = amount[x];
                double balance = 0;
                var Cust_balance = db.sp_Get_Customer_with_Balance2(ID).FirstOrDefault();
                if (Cust_balance != null)
                {
                    balance = Cust_balance.Total;
                }
                string inwords = helper.mnc.ToWords(Mantainance.ToString());
                string strTran_Nbr = ID + DateTime.UtcNow.AddHours(3).ToString().GetHashCode().ToString("x");
                string tranNbr = ID + DateTime.UtcNow.AddHours(3).ToString().GetHashCode().ToString("x");
                int? Cust_Tran_ID = db.tblCustomer_Transaction.Where(a => a.CustomerID == ID && a.Status == "Open").Select(a => a.Cust_Tran_ID).FirstOrDefault();
                if (Cust_Tran_ID == null || Cust_Tran_ID==0)
                {
                    db.tblCustomer_Transaction.Add(new Models.tblCustomer_Transaction
                    {
                        Amount = Mantainance,
                        CustomerID = ID,
                        Tran_Date = trandate,
                        Status = "Open",
                        UserID = UserID,

                    });

                    db.SaveChanges();
                    Cust_Tran_ID = db.tblCustomer_Transaction.Select(a => a.Cust_Tran_ID).Max();

                }
                else
                {

                    try
                    {
                        var tran = db.tblCustomer_Transaction.Where(a => a.CustomerID == ID && a.Status == "Open").FirstOrDefault();
                        float? balance2 = tran.Amount;

                        tran.Amount = (float)(balance + Mantainance);
                        db.SaveChanges();

                    }
                    catch (Exception ex)
                    {

                    }

                }

                int Cust_Tran_Type = 0;
                Cust_Tran_Type = db.tblCustomer_Tran_Type.Where(a => a.Tran_Type == "Customer Invoice").Select(a => a.Cust_Tran_Type_ID).FirstOrDefault();
             
                db.tblCustomer_Transaction_Detail.Add(new Models.tblCustomer_Transaction_Detail
                {
                    Credit = 0,
                    Debit = Mantainance,
                    Cust_Tran_ID = (int)Cust_Tran_ID,
                    Description = "Mantainance",
                    Trans_Date = trandate,
                    Cust_Tran_Type_ID = Cust_Tran_Type,
                    Tran_Nbr = strTran_Nbr,
                    UserID = UserID,
                    Unit_price = Mantainance,
                    Qty = 1,
                    Invoice_No = serialno,
                    Route = inwords,
                    Status= "Open",
                    Balance=(float)balance
                });

                db.SaveChanges();
                float intTotal_Vendor_Balance = (float)db.Get_Total_Customer_Transaction_Detail_by_TranID(Cust_Tran_ID).FirstOrDefault();
                db.Update_Customer_Tranaction_Amount_By_Cust_tran_id(intTotal_Vendor_Balance, Cust_Tran_ID);

                var customername = db.tblCustomers.Where(a => a.CustomerID == ID).Select(a => a.Customer_Name).FirstOrDefault();

                // Add Journal

                db.tblJournals.Add(new Models.tblJournal
                {
                    Journal_Date = trandate,
                    Tran_Nbr = strTran_Nbr,
                    TYear = trandate.Year,
                    Period = trandate.Month,
                    UserID = UserID,
                    Status = "Open",
                    Tran_Type = "Customers",
                    Description = "Customer Post",
                });
                db.SaveChanges();

                int Journal_id = db.tblJournals.Where(a => a.UserID == UserID).Select(a => a.Journal_id).Max();

                // AR
                db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = Mantainance, Credit = 0, Account_Number = 1150 });
                db.SaveChanges();
                // Income
                db.tblJournal_Details.Add(new Models.tblJournal_Details { Journal_id = Journal_id, Debit = 0, Credit = Mantainance, Account_Number = 3130 });
                db.SaveChanges(); int Journal_Detail_id = db.tblJournal_Details.Where(a => a.Journal_id == Journal_id).Select(a => a.Journal_Detail_id).Max();


                var y = db.tblInvoices.FirstOrDefault();
                if (y != null)
                {
                    y.Invoice_No = serialno;
                    db.SaveChanges();
                }
                i += 1;
                serialno += 1;


            }



 
            return RedirectToAction("CustomerPost", "Customers", new { msg = "Customer Posted Successfully for " + i + " Customer(s)" });

        }
    }
}