using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SameSoftWeb.Controllers
{
    public class WebController : Controller
    {
        // GET: Web
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AllCust()
        {
            return View();
        }
        public ActionResult Hotelsandrestaurant()
        {
            return View();
        }
        public ActionResult Hospitals()
        {
            return View();
        }

        public ActionResult AgentsandAirlines()
        {
            return View();
        }

        public ActionResult University()
        {
            return View();
        }

        public ActionResult CustomSoftware()
        {
            return View();
        }
    }
}