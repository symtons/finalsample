using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SameSoftWeb.helper
{
    public class sitemethods
    {

       
        public static DateTime getdate()
        {


            int var = 0; // change  timezone offset here
            var = 3; // east africa
          

            DateTime d = DateTime.UtcNow.AddHours(var);


            return d;
        }



        public static    void addlog(string title,string details,int? div,int userid)
        {
            SameSoftWeb.Models.SameSoftwareWebEntities db = new SameSoftWeb.Models.SameSoftwareWebEntities();

            DateTime dt = DateTime.UtcNow.AddHours(3);

            db.TblLogs.Add(new Models.TblLog { LogDate = dt, Title = title, Details = details, UserID = userid});
            db.SaveChanges();

        }


        public static String fileNameCleaner(String datei)
        {
            if (datei != null)
            {
                datei = datei.Replace("ß", "ss");
                datei = datei.Replace("ä", "ae");
                datei = datei.Replace("ö", "oe;");
                datei = datei.Replace("ü", "ue");
                datei = datei.Replace("Ü", "Ue");
                datei = datei.Replace("Ö", "Oe");
                datei = datei.Replace("Ä", "Ae");
                datei = datei.Replace("!", "");
                datei = datei.Replace("?", "");
                datei = datei.Replace("/", "");
                datei = datei.Replace('"', '_');
                datei = datei.Replace(" ", "_");
                datei = datei.Replace("-", "");
                datei = datei.Replace(">", "");
                datei = datei.Replace("<", "");
                datei = datei.Replace("=", "");
                datei = datei.Replace("$", "");
                datei = datei.Replace("§", "");
                datei = datei.Replace("}", "");
                datei = datei.Replace("{", "");
                datei = datei.Replace("*", "");
                datei = datei.Replace("+", "");
                datei = datei.Replace("#", "");
                datei = datei.Replace("~", "");
                datei = datei.Replace(":", "");
                datei = datei.Replace("`", "");
                datei = datei.Replace(";", "");
                datei = datei.Replace("®", "");
                datei = datei.Replace("®", "");
                datei = datei.Replace(",", "");
                datei = datei.Replace("'", "");
                datei = datei.Replace("“", "");
                datei = datei.Replace("Â", "");
                datei = datei.Replace("é", "e");
                datei = datei.Replace("á", "a");
                datei = datei.Replace("„", "");
                datei = datei.Replace("#", "");
                datei = datei.Replace("|", "");
                datei = datei.Replace("â", "");
                datei = datei.Replace("€", "");
                datei = datei.Replace("“", "");
                datei = datei.Replace("´", "");
                datei = datei.Replace("&", "");
                datei = datei.Replace("____", "_");
                datei = datei.Replace("___", "_");
                datei = datei.Replace("__", "_");

                if (datei.Substring(datei.Length - 2, 2) == "__")
                {
                    datei = datei.Substring(0, datei.Length - 2);
                }




                if (datei.Length > 80)
                {
                    datei = datei.Substring(0, 79);
                }
                return datei;
            }
            else
            {
                return null;
            }
        }


    }
}