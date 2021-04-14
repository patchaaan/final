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
    public class LoginController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(tblStaff tblstaff)
        {
            var checkLogin = db.tblStaffs.Where(x => x.Username.Equals(tblstaff.Username) && x.Password.Equals(tblstaff.Password)).FirstOrDefault();
            if (checkLogin != null)
            {
                Session["isAAG"] = checkLogin.isAAG.ToString();
                Session["isADG"] = checkLogin.isADG.ToString();
                Session["isEDG"] = checkLogin.isEDG.ToString();
                Session["isPG"] = checkLogin.isPG.ToString();
                Session["Username"] = tblstaff.Username.ToString();

                //if (checkLogin.isAAG.ToString() == "Y") {
                //    Session["isAAG"] = true;
                //}
                //if (checkLogin.isADG.ToString() == "Y") {
                //    Session["isADG"] = true;
                //}
                //if (checkLogin.isEDG.ToString() == "Y") {
                //    Session["isEDG"] = true;

                //}
                //if (checkLogin.isPG.ToString() == "Y") {
                //    Session["isPG"] = true;
                //}
                return RedirectToAction("Index", "Residents");

            }
            else {
                ViewBag.Notification = "Invalid Credentials!";
            }

            return View();
        }

    }
}