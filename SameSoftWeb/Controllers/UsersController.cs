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
    public class UsersController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.tblUsers.ToList());
        }

        // GET: Users/Details/6
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUser tblUser = db.tblUsers.Find(id);
            if (tblUser == null)
            {
                return HttpNotFound();
            }
            return View(tblUser);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,FullName,Customer_ID,Role,UserName,Password,Email,Status")] tblUser tblUser)
        {
            if (ModelState.IsValid)
            {
                db.tblUsers.Add(tblUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblUser);
        }

        // GET: Users/Edit/6
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUser tblUser = db.tblUsers.Find(id);
            if (tblUser == null)
            {
                return HttpNotFound();
            }
            return View(tblUser);
        }

        // POST: Users/Edit/6
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317698.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,FullName,Customer_ID,Role,UserName,Password,Email,Status")] tblUser tblUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblUser);
        }

        // GET: Users/Delete/6
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUser tblUser = db.tblUsers.Find(id);
            if (tblUser == null)
            {
                return HttpNotFound();
            }
            return View(tblUser);
        }

        // POST: Users/Delete/6
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblUser tblUser = db.tblUsers.Find(id);
            db.tblUsers.Remove(tblUser);
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

        public ActionResult Login()
        {

            return View();

        }



        public ActionResult Logout()
        {
            Session["UserID"] = null;
            Session["UserName"] = null;
            Session["Role"] = null;
            Session["FullName"] = null;
            Session["AppID"] = null;
            Session["customer_id"] = null;
            return RedirectToAction("Index","Home");
        }


        public ActionResult mylogin(string username, string password)
        {

            bool success = false;
            bool err = false;
            bool blocked = false;

            try
            {
                // var db = new WebAirLine.Models.Shabele_Airline_Data_NewEntities();
                var db = new SameSoftWeb.Models.SameSoftwareWebEntities();
                //UTF8Encoding encoder = new UTF8Encoding();
                //MD6CryptoServiceProvider hasher = new MD6CryptoServiceProvider();
                //string hash = System.Convert.ToBase64String(hasher.ComputeHash(encoder.GetBytes(password)));

                int? tmp = db.tblUsers.Where(a=>a.UserName==username && a.Password==password).Select(a=>a.UserID).FirstOrDefault();
                if (tmp != null)
                {

                    var resUser = (from user in db.tblUsers where user.UserID == tmp select user).First();

                   // if (resUser.Status.ToString().ToLower().Contains("active") == false)
                   // {

                    //    return Json("failed", JsonRequestBehavior.AllowGet);

                        //  return Json("Login Failed ; this User is  Blocked  !", JsonRequestBehavior.AllowGet);
                  //  }

                    Session["UserID"] = resUser.UserID;
                    Session["UserName"] = resUser.UserName;
                    Session["Role"] = resUser.Role;
                    Session["FullName"] = resUser.FullName;
                    //Session["UserAgent_ID"] = db.Sp_Get_Agent_id_by_UserID(resUser.UserID).FirstOrDefault();
                    //Session["Login_Name"] = resUser.Login_Name;



                    //// Get User Access MENUS
                    //List<int> menu_acc = new List<int>();

                    //List<WebAirLine.Models.Get_User_Menu_By_UserID_Result_mnc> collection;
                    //collection = db.Get_User_Menu_By_UserID(resUser.UserID).ToList();

                    //for (int r = 0; r < collection.Count; r++)
                    //{
                    //    menu_acc.Add(collection[r].MENUID);
                    //}

                    //Session["menu_acc"] = "---";
                    //Session["menu_acc"] = menu_acc;

                    // END OF GET USER ACCESS MENUS


                    //

                    //var agentinfo = SiteMethods.GetAgentInfoByAgentUserId((int)Session["UserID"]);
                    //if (agentinfo != null)
                    //{
                    //    Session["Login_Name"] = agentinfo.Agent_Name;
                    //    Session["Agent_Name"] = agentinfo.Agent_Name;
                    //    Session["UserAgent_ID"] = agentinfo.Agent_id;
                    //}
                    //else
                    //{
                    //    Session["Login_Name"] = "NA";
                    //    Session["UserAgent_ID"] = db.Sp_Get_Agent_id_by_UserID(resUser.UserID).FirstOrDefault();
                    //}
                    ////
                    success = true;
                    if (resUser.Role == "Customer")
                    {
                        Session["AppID"] = resUser.App_ID;
                        Session["customer_id"] = resUser.Customer_ID;

                        Session["Customer_Name"] =  db.tblCustomers.Where(a=>a.CustomerID==resUser.Customer_ID).Select(a=>a.Customer_Name).FirstOrDefault();
                        return Json("Customer/Index", JsonRequestBehavior.AllowGet);

                    }
                    else if (resUser.Role=="Admin")
                    {
                        return RedirectToAction("Index", "Admin");

                        //return Json("Admin/Index", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Admin");
                        //return Json("Admin/Index", JsonRequestBehavior.AllowGet);
                        // return Json("Staff/Index", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Database error. " + e.Message);
                err = true;
            }

            if (!success)
            {

                Session["UserID"] = null;
                Session["Role"] = null;
                Session["UserName"] = null;
                Session["FullName"] = null;

                if (err)
                {
                    return RedirectToAction("Login", "Users", new { msg = "Invalid UserName Or Password" });
                    //return Json("failed", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    {
                        return RedirectToAction("Login", "Users", new { msg = "Invalid UserName Or Password" });
                        //return Json("failed", JsonRequestBehavior.AllowGet);
                        //  return Json("Login Failed. check user & Password", JsonRequestBehavior.AllowGet);
                    }
                }


            }

            //ViewBag.msg = "Invalid UserName Or Password";

            return RedirectToAction("Login", "Users", new { msg = "Invalid UserName Or Password" });
           // return Json("failed", JsonRequestBehavior.AllowGet);
        }




    }
}
