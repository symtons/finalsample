using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SameSoftWeb.Controllers
{
    public class DocumentsController : Controller
    {
        // GET: Documents
        public ActionResult Index()
        {
            return View();
        }


              public ActionResult Documents()
        {
            
            return View();
        }
        public ActionResult DocumentsReport(DateTime? from, DateTime? to)
        {
            ViewBag.from = from;
            ViewBag.to = to;

            return View();
        }
        public ActionResult Print(int? id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult UploadPhoto(string name, string refno, DateTime tran_date, int id, HttpPostedFileBase file)
        {

            if (file != null)
            {
                if (file.FileName.Contains(".jpg") == false)
                {
                    Session["err"] = "Please Upload .JPG file Only";
                    return RedirectToAction("Print", "Documents", new { id = id });
                }
                else
                {
                    string path = Server.MapPath("~/photo/Documents");

                    file.SaveAs(path + "/" + id + ".jpg");
                    ViewData["path"] = id + ".jpg";
                    using (var db = new SameSoftWeb.Models.SameSoftwareWebEntities())
                    {
                        var x = db.TblDocuments.Add(new Models.TblDocument
                        {
                            Documents = "/Photo/Documents/" + id + ".jpg",
                            Cust_Name = name,
                            RefNo = refno,
                            Tran_Date = tran_date
                        });
                        db.SaveChanges();
                    }

                }



            }
            else
            {

                Session["err"] = "No File Selected";
                return RedirectToAction("Print", "Documents", new { id = id });
            }
            return RedirectToAction("Print", "Documents", new { id = id });

        }


     

        public ActionResult employeeCapture()
        {
            var stream = Request.InputStream;
            string dump;

            using (var reader = new StreamReader(stream))
            {
                dump = reader.ReadToEnd();

                DateTime nm = DateTime.Now;

                //  string date = nm.ToString("yyyymmddMMss");

                var id = Session["employeephotoid"];

                var path = Server.MapPath("~/Photo/Documents/" + id + ".jpg");

                System.IO.File.WriteAllBytes(path, String_To_Bytes2(dump));

                int idx = id == null ? 0 : (int)id;


                //using (var db = new SameSoftWeb.Models.SameSoftwareWebEntities())
                //{
                //    var Cust = db.Forex_Customers.Where(a => a.Customer_id == idx).FirstOrDefault();
                //    if (Cust != null)
                //    {
                //        Cust.Photo = "/Photo/Customer/" + id + ".jpg";
                //        db.SaveChanges();
                //    }
                //}



                ViewData["path"] = id + ".jpg";

                Session["val"] = id + ".jpg";
            }

            return View("Index");
        }




        private byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;
            byte[] bytes = new byte[numBytes];
            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }
            return bytes;
        }




        //public JsonResult Rebind()
        //{
        //    string id = Session["studentphotoid"].ToString();
        //    var path = "/Photo/Customer/" + id + ".jpg";
        //    return Json(path, JsonRequestBehavior.AllowGet);
        //}


        public JsonResult Rebind2()
        {
            string id = Session["employeephotoid"].ToString();
            var path = "/Photo/Documents/" + id + ".jpg";
            return Json(path, JsonRequestBehavior.AllowGet);
        }

    
    }
}