using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SameSoftWeb.Controllers
{
    public class ImportSamplesController : Controller
    {
        // GET: ImportSamples
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Download(string file)
        {

            string fullpath= Server.MapPath("~/ImportSamples/") + file;

            byte[] fileBytes = System.IO.File.ReadAllBytes(fullpath);
            string fileName = file;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            
        }

        public ActionResult University()
        {
            return View();
        }

        public ActionResult TravelAgents()
        {
            return View();
        }







    }
}