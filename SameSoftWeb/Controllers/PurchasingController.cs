using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SameSoftWeb.Controllers
{


    public class PurchasingController : Controller
    {
        SameSoftWeb.Models.SameSoftwareWebEntities db = new SameSoftWeb.Models.SameSoftwareWebEntities();
        // GET: Purchasing
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PurchaseRequest()
        {
            return View();
        }
        [HttpPost]
        public ActionResult add_purchase_request(int[] item_id, float[] qty, float[] unit_cost, int vendor_id, string note,DateTime? trandate)
        {
            int userid = (int)Session["UserID"];
            int business_division = 1;


            DateTime vdate = SameSoftWeb.helper.sitemethods.getdate();
            if (trandate != null)
            {
                DateTime dt = (DateTime)trandate;
                if (dt.Date == vdate.Date)
                {
                    vdate = SameSoftWeb.helper.sitemethods.getdate();
                }
                else
                {
                    vdate = dt;
                }
            }

            // Validate
            db.Database.BeginTransaction();
            db.tblPurchase_Order.Add(new Models.tblPurchase_Order { Business_Division_Id = business_division, Total = 0, Tran_Date = vdate, Status = "Open", UserID = userid, Note = note, Vendor_Id = vendor_id });
            db.SaveChanges();
            int pur_id = 0;
            pur_id = db.tblPurchase_Order.Select(a => a.Purchase_ID).Max();

            for (int x = 0; x <= item_id.Length - 1; x++)
            {
               db.tblPurchase_Order_Details.Add(new Models.tblPurchase_Order_Details { Purchase_ID = pur_id, Item_id = item_id[x], Qty = qty[x], Unit_Cost = unit_cost[x] });
             //   db.tblPurchase_Order_Details.Add(new Models.tblPurchase_Order_Details { Purchase_ID = pur_id, Item_id = item_id[x], Qty = qty[x] });


                //// Update Item Price / Cost
                //int id = item_id[x];
                //float costx = unit_cost[x];
                //var temp = db.Items.Where(a => a.Item_Id == id).FirstOrDefault();
                //if (temp!=null)
                //{
                //    temp.Sale_Price = (decimal)costx;
                //    db.SaveChanges();
                //}


            }
            db.SaveChanges();

            float total = 0;
            total = (float)db.tblPurchase_Order_Details.Where(a => a.Purchase_ID == pur_id).Select(a => a.Qty * a.Unit_Cost).Sum();
            var p = db.tblPurchase_Order.Where(f => f.Purchase_ID == pur_id).FirstOrDefault();
            p.Total = total;
            db.SaveChanges();


            db.Database.CurrentTransaction.Commit();

            //str = str + " All data Insetred";


            return RedirectToAction("ReceivePurchase", "Purchasing");

            //  return RedirectToAction("success", "message", new { msg = "Purchase Request Completed Successfully !" });
        }



        public ActionResult ReceivePurchase()
        {
            return View();
        }




        public ActionResult Receive_Purchase(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ReceivePurchase");
            }

            ViewBag.id = id;
            return View();
        }



        public ActionResult add_inventory(int[] item_id, float[] cost, float[] price, float[] qty, string[] batch, string[] barcode, DateTime?[] manudate, DateTime?[] expiredate, int purchase_id, float Freight, float Tax, DateTime? trandate)
        {
            //UsersController u = new UsersController();
            //u.checkaccess(22);

            try
            {

                int userid = (int)Session["UserID"];
                int business_division = 1;

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
                var purchasetbl = db.tblPurchase_Order.Where(a => a.Purchase_ID == purchase_id).FirstOrDefault();
                var vendor = purchasetbl.Vendor_Id;

                int inventory_type_id = (int)db.Inventory_Types.Where(a => a.Inventory_Type == "Purchase").Select(a => a.Inventory_type_id).FirstOrDefault();


                db.tblInventory_Transactions.Add(new Models.tblInventory_Transactions { Business_Division_Id = business_division, Total = 0, Tran_Date = vdate, Status = "Open", UserID = userid, Payment_Method = "N/A", Inventory_type_id = inventory_type_id, Tran_Nbr = "TRAN123", Vendor_ID = vendor });
                db.SaveChanges();


                int serialno = 0;
                int? sr = db.tblInventory_Transactions.Where(a => a.Business_Division_Id == business_division && a.Inventory_type_id == inventory_type_id).Select(a => a.Invoice_No).Max();
                if (sr == null)
                {
                    serialno = int.Parse(db.tblSettings.Where(a => a.ID == 2).Select(a => a.Value).FirstOrDefault());
                }
                else
                {
                    serialno = (int)sr + 1;
                }



                int last_inventory_id = 0;
                last_inventory_id = db.tblInventory_Transactions.Select(a => a.Inventory_Tran_ID).Max();

           

                //Get Total Cost
                float TotalCost = 0;
                float freight_rate = 0;
                float tax_rate = 0;
                for (int x = 0; x <= item_id.Length - 1; x++)
                {
                    TotalCost = TotalCost + (cost[x] * qty[x]);
                }

                if (Freight > 0)
                {
                    freight_rate = Freight / TotalCost;
                }


                if (Tax > 0)
                {
                    tax_rate = Tax / TotalCost;
                }




                for (int x = 0; x <= item_id.Length - 1; x++)
                {
                    int itemid = item_id[x];
                    // Update Item Price / Cost
                    float costx = cost[x];
                    float pricex = price[x];




                    float per_freight = 0;
                    float per_tax = 0;
                    float t_cost = 0;
                    t_cost = cost[x] * qty[x];
                    per_freight = freight_rate * t_cost;
                    per_tax = tax_rate * t_cost;

                    float per_freight_qty = per_freight / qty[x];  // Devide the Freight per 1 item
                    float per_tax_qty = per_tax / qty[x];  // Devide the Tax per 1 item


                    db.tblInventory_Transaction_Details.Add(new Models.tblInventory_Transaction_Details { Inventory_Tran_ID = last_inventory_id, Item_Id = item_id[x], Qty_In = qty[x], Qty_Out = 0, Unit_Price = cost[x], Sales_price = price[x], Batch = batch[x], Freight = per_freight_qty, Tax = per_tax_qty, Barcode = barcode[x] });
                    db.SaveChanges();


                    var temp = db.Items.Where(a => a.Item_Id == itemid).FirstOrDefault();
                    if (temp != null)
                    {
                        temp.Sale_Price = (decimal)pricex;
                        decimal CostItem = (decimal)cost[x];
                        float AvailableQty = (float)db.Inv_Get_Qty_Available_by_ProductID(itemid).FirstOrDefault();
                        float Available = AvailableQty - qty[x];

                        // MAKE AVARAGE COST
                        if (Available > 0)
                        {
                            decimal? CostOfItem = db.Items.Where(a => a.Item_Id == itemid).Select(a => a.Unit_Cost).FirstOrDefault();
                            decimal? totalCostOfItem = CostOfItem + CostItem;
                            decimal avg_cost = (decimal)(totalCostOfItem / 2);

                            temp.Unit_Cost = avg_cost;
                            db.SaveChanges();
                        }
                        //int NumberOfTimesPurchased = db.tblInventory_Transaction_Details.Where(a => a.Item_Id == itemid && a.Qty_In > 0).Select(a => a.Item_Id).Count();
                        //float totalCostOfItem = db.tblInventory_Transaction_Details.Where(a => a.Item_Id == itemid && a.Qty_In > 0).Select(a => a.Unit_Price).Sum();
                        //decimal avg_cost = (decimal)(totalCostOfItem / NumberOfTimesPurchased);

                        if (Available == 0)
                        {
                            temp.Unit_Cost = CostItem;
                            db.SaveChanges();
                        }
                        // MAKE AVARAGE COST


                    }









                    if (manudate != null && expiredate != null)
                    {
                        int last_inventory_Detail_id = db.tblInventory_Transaction_Details.Select(a => a.Inventory_Tran_Detail_ID).Max();
                        db.tblBatches.Add(new Models.tblBatch { Batch = batch[x], Manufacture_Date = manudate[x], Expire_Date = expiredate[x], Inventory_Detail_id = last_inventory_Detail_id });
                    }

                }
                db.SaveChanges();

                float total = 0;

                total = (float)db.tblInventory_Transaction_Details.Where(a => a.Inventory_Tran_ID == last_inventory_id).Select(a => a.Qty_In * (a.Unit_Price + a.Freight + a.Tax)).Sum();
                var p = db.tblInventory_Transactions.Where(f => f.Inventory_Tran_ID == last_inventory_id).FirstOrDefault();

                p.Invoice_No = serialno;
                p.Total = total;
                // Updatete Fregit
                purchasetbl.Status = "Received";
                db.SaveChanges();



                db.Database.CurrentTransaction.Commit();

                //add log
                helper.sitemethods.addlog("Add Data", "Purchase Received Inv# (" + serialno + ") ", business_division, userid);

                return RedirectToAction("PostPurchase", "Accounting");
            }
            catch (Exception ex)
            {
                db.Database.CurrentTransaction.Rollback();
                throw ex;
            }
        }




        public ActionResult Delete_Request(int id)
        {
            // check user access for this action...

            var pur = db.tblPurchase_Order.Where(a => a.Purchase_ID == id).FirstOrDefault();
            if (pur != null)
            {
                if (pur.Status.Trim() == "Open")
                {
                    db.tblPurchase_Order.Remove(pur);
                    db.SaveChanges();
                    var dt = db.tblPurchase_Order_Details.Where(a => a.Purchase_ID == id).ToList();
                    db.tblPurchase_Order_Details.RemoveRange(dt);
                    db.SaveChanges();
                    return RedirectToAction("View_Msg", "Home", new { msg = "Purchase Request Deleted Successfully !" });
                }

            }

            return RedirectToAction("View_Msg", "Home", new { msg = "Purchase Request Not Deleted !" });

        }


        public ActionResult CancelPurchase(int id)
        {
            var inv = db.tblInventory_Transactions.Where(a => a.Inventory_Tran_ID == id).FirstOrDefault();

            if (inv != null)
            {
                inv.Status = "Cancelled";
                db.SaveChanges();
                return RedirectToAction("View_Msg", "Home", new { msg = "Purchase  Cancelled Successfully !" });

            }

            return RedirectToAction("View_Msg", "Home", new { msg = "Purchase  Not Cancelled !" });

        }


        public ActionResult Receipt(int? id)
        {
            //if (id == null)
            //{
            //    return RedirectToAction("Dashboard", "Home");

            //}

            ViewBag.id = id;
            return View();
        }


        public ActionResult RequestLetter(int? id)
        {
            ViewBag.id = id;
            return View();
        }


        public ActionResult UpdateBarcodeLot(int id,string lot,string barcode)
        {


            var p = db.tblInventory_Transaction_Details.Where(a => a.Inventory_Tran_Detail_ID == id).FirstOrDefault();
            if (p != null)
            {
                p.Batch = lot;
                p.Barcode = barcode;
                db.SaveChanges();
                return RedirectToAction("View_Msg", "Home", new { msg = "Inventory barcode and lot No. Updated Successfully !" });

                var batch = db.tblBatches.Where(a => a.Inventory_Detail_id == id).ToList();
                if(batch!=null)
                {
                    batch.ForEach(a => a.Batch = lot);
                    db.SaveChanges();
                }
           }
            else
            {
                return RedirectToAction("View_Msg", "Home", new { msg = "Inventory Not Updated !" });

            }



            return View();


        }


        public ActionResult InvetoryOut(int? Item_id_list)
        {
            ViewBag.item = Item_id_list;
            return View();
        }


        public ActionResult SaveInventoryOut( float Qty_Out,int ID, string name, float cost, string note, string Batch)
        {

            float totalcost = Qty_Out * cost;
            int last_inventory_id = 0;
            last_inventory_id = db.tblInventory_Transactions.Select(a => a.Inventory_Tran_ID).Max();

            db.tblInventory_Transaction_Details.Add(new Models.tblInventory_Transaction_Details { Inventory_Tran_ID = last_inventory_id, Item_Id = ID, Qty_In = 0, Qty_Out = Qty_Out, Unit_Price = cost, Sales_price = 0, Batch = Batch });
        
            db.SaveChanges();

            DateTime trandate = DateTime.UtcNow.AddHours(3).Date;

            int userid = (int)Session["UserID"];

            string strTran_Nbr = DateTime.UtcNow.AddHours(3).ToString().GetHashCode().ToString("x");

            db.tblExpenses.Add(new Models.tblExpense { Expense_Type_ID = 2, Cr = totalcost, Expense_Date = trandate, UserID = userid, Period = trandate.Month, Year = trandate.Year, Note = note, Status = "Open", Tran_Nbr = strTran_Nbr });
            db.SaveChanges();

            string typename = db.tblExpenseTypes.Where(a => a.Expense_Type_ID == 2).Select(a => a.Expense_Type).FirstOrDefault();

            helper.mnc.addlog("Expense", "Added New Expense of type  " + typename + " Amonut= " + totalcost, userid);

            ViewBag.msg = "Expense Added Successfully";
            return View("InvetoryOut");
        }
       



      


    }
}