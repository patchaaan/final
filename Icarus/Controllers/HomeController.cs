using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Icarus.Models;

namespace Icarus.Controllers
{
    public class HomeController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Route("ChangePassword")]
        [HttpPost, ActionName("ChangePassword")]
        public ActionResult ChangePassword(string newPassword)
        {
            if (Session["Username"] != null)
            {
                tblStaff staff = db.tblStaffs.Find(Int16.Parse(Session["ID"].ToString()));
                staff.Password = newPassword;
                db.Entry(staff).State = EntityState.Modified;
                db.SaveChanges();
                var success = "Success";
                return Content(success);
            }
            else {
                return RedirectToAction("Login","Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Login", "Login");
        }
    }
}