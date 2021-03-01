using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SameSoftWeb.Helpers
{
    public class UserAccess
    {
        public static List<int> Menu_Access = new List<int>();

        public static List<Menu_list> menulist = new List<Menu_list>();
        public bool Get_user_Access(int userid)
        {
            int UserID = 0;
            var db = new SameSoftWeb.Models.SameSoftwareWebEntities();
 //menulist = db.Get_User_Menu_By_UserID(UserID)
            //                   .Select(tc => new Menu_list
            //                   {
            //                       MenuID = tc.MenuID,
            //                       Menu_Name = tc.Menu_Name
            //                   })
            //                   .ToList();

            return true;




        }



    }

    public class Menu_list
    {


        public int MenuID { get; set; }
        public string Menu_Name { get; set; }

    }


}