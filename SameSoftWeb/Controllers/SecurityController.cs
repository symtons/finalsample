using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Encrypt;
using System.Web.Mvc;
using System.Text;
using SameSoftWeb.Models;
using System.Net;

namespace SameSoftWeb.Controllers
{
    public class SecurityController : Controller
    {
        // GET: Security
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();

        public ActionResult GenerateConnectionString()
        {
            byte[] data = Request.BinaryRead(Request.ContentLength);

           
            string cleandata = System.Text.ASCIIEncoding.ASCII.GetString(data);
            cleandata= cleandata.Replace("Data=", "");

            string auth = cleandata.Split('*')[0];
            string con = cleandata.Split('*')[1];

            if(auth!="1")
            { return Content("Authentication-Error"); }

           
           return Content(AsyncEnc.EncryptData(con));
        }


        public ActionResult dec101(string s)
        {
            return Content(AsyncEnc.DecryptData(s));
        }



        public ActionResult GetPlainPreparedConnection(int? id,string auth)
        {
            

            
            if (auth != "1")
            { return Content("Authentication-Error"); }

            string Connection_String = "";
            var conn_row = db.tblConnection_Data.Where(a => a.Connection_ID == id).FirstOrDefault();
            if (conn_row != null)
            {
                Connection_String = conn_row.Database_Server + "~" + conn_row.Database_User + "~" + conn_row.Database_Password + "~" + conn_row.Database_Name;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return Content(Connection_String);
            //  return Content(auth+ matchine+ customer);
        }
        public ActionResult GetPreparedConnection() 
        {
            byte[] data = Request.BinaryRead(Request.ContentLength);


            string cleandata = System.Text.ASCIIEncoding.ASCII.GetString(data);
            cleandata = cleandata.Replace("Data=", "");


        


            string auth = cleandata.Split('*')[0];
            string matchine = cleandata.Split('*')[1];
            int connection_id = Int32.Parse(cleandata.Split('*')[2]);
            if (auth != "1")
            { return Content("Authentication-Error"); }

            string Connection_String = "";
            var conn_row = db.tblConnection_Data.Where(a => a.Connection_ID == connection_id).FirstOrDefault();
            if(conn_row!=null)
            {
                Connection_String = matchine+"~"+conn_row.Database_Server+"~"+conn_row.Database_User+"~"+conn_row.Database_Password+"~"+conn_row.Database_Name;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return Content(AsyncEnc.EncryptData(Connection_String));
          //  return Content(auth+ matchine+ customer);
        }


        public ActionResult GetConnectionString(string serial,string str_server,string db,string user,string pass)
        {
            int user_id = (int)Session["UserID"];

            string connection = "";

            if(string.IsNullOrEmpty(serial)==false)
            {

               

                string fulltext = serial + "~" + str_server  + "~" + user + "~" + pass+"~" + db +"~ Generated_at_:" +DateTime.UtcNow.AddHours(3)+"_from_Web_by:"+user_id;
           
                connection = Encryptor.Encrypt(fulltext, true);
                ViewBag.connection = connection;
            }


            return View();
        }


        public ActionResult Connections()
        {

            return View();
        }

        public ActionResult downloadConnection(string conn)
        {
            var contentType = "text/xml";
           
            var bytes = Encoding.UTF8.GetBytes(conn);
            var result = new FileContentResult(bytes, contentType);
            result.FileDownloadName = "SameSoft.con";
            return result;
        }


    }
}