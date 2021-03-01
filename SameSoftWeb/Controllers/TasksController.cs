using SameSoftWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SameSoftWeb.Controllers
{
    public class TasksController : Controller
    {
        private SameSoftwareWebEntities db = new SameSoftwareWebEntities();
        // GET: Tasks
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Create()
        {
            return View();
        }


        public ActionResult Update(int id)
        {
            ViewBag.task_id = id;
            return View();
        }

        [HttpPost]
       public ActionResult Create(string status,string Dep, int user_id,string title,string desc,int? customer_id,int? project_id,string from,string to,string priority)
        {

            DateTime vdate = DateTime.UtcNow.AddHours(3);

            int c_user_id = 0;

            db.tblTasks.Add(new tblTask
            {
                Assigned_for = user_id,
                Created_by = c_user_id,
                Creation_Date = vdate,
                Customer_id = customer_id,
                Description = desc,
                Start_Date = from,
                End_Date = to,
                Priority = priority,
                Status = status,
                Title = title,
                Department=Dep,
            });

            db.SaveChanges();

  return RedirectToAction("Index","Tasks",new {msg="Task Created Successfully " });
        }



        [HttpPost]
        public ActionResult Update(int task_id,string status, string Dep, int user_id, string title, string desc, int? customer_id, int? project_id, string from, string to, string priority)
        {

            DateTime vdate = DateTime.UtcNow.AddHours(3);


            var task = db.tblTasks.Where(a => a.TaskID == task_id).FirstOrDefault();

            if(task==null)
            {
                return Content("Task Not Found");
            }

            int c_user_id = 0;


            task.Assigned_for = user_id;
            task.Created_by = c_user_id;
                task.Last_Update = vdate;
            task.Department = Dep;
                task.Customer_id = customer_id;
                task.Description = desc;
                task.Start_Date = from;
                task.End_Date = to;
            task.Priority = priority;
                task.Status = status;
                task.Title = title;
        

            db.SaveChanges();

            return RedirectToAction("Index", "Tasks", new { msg = "Task Updated Successfully " });
        }

    
}
}