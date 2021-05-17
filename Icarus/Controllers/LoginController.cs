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
            if (Session["Username"] != null)
            {
                return RedirectToAction("Index", "Residents");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["Username"] != null) {
                return RedirectToAction("Index", "Residents");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(tblStaff tblstaff)
        {
            var checkLogin = db.tblStaffs.Where(x => x.Username.Contains(tblstaff.Username) && x.Password.Contains(tblstaff.Password)).FirstOrDefault();
            if (checkLogin != null)
            {
                if (tblstaff.Username == checkLogin.Username)
                {
                    if (tblstaff.Password == checkLogin.Password)
                    {
                        Session["isAAG"] = checkLogin.isAAG.ToString();
                        Session["isADG"] = checkLogin.isADG.ToString();
                        Session["isEDG"] = checkLogin.isEDG.ToString();
                        Session["isPG"] = checkLogin.isPG.ToString();
                        Session["Username"] = tblstaff.Username.ToString();
                        Session["Password"] = checkLogin.Password.ToString();
                        Session["ID"] = checkLogin.IDStaff.ToString();
                        return RedirectToAction("Index", "Residents");
                    }
                    else {
                        ViewBag.Notification = "Invalid Password!";
                    }
                }
                else {
                    ViewBag.Notification = "Invalid Username!";
                }
            }
            else {
                ViewBag.Notification = "Invalid Credentials!";
            }

            return View();
        }

    }
}