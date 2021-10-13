using System;
using System.Data.Entity;
using System.Web.Mvc;
using Icarus.Models;

namespace Icarus.Controllers
{
    public class HomeController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        [Route("ChangePassword")]
        [HttpPost, ActionName("ChangePassword")]
        public ActionResult ChangePassword(string newPassword)
        {
            if (Session["Username"] != null)
            {
                try
                {
                    tblStaff staff = db.tblStaffs.Find(Int16.Parse(Session["ID"].ToString()));
                    staff.Password = newPassword;
                    db.Entry(staff).State = EntityState.Modified;
                    db.SaveChanges();
                    var success = "Success";
                    return Content(success);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }
                return HttpNotFound();
            }
            else {
                return RedirectToAction("Login","Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            try
            {
                Session.Clear();
                Session.RemoveAll();
                Session.Abandon();
                return RedirectToAction("Login", "Login");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception " + e);
            }
            return HttpNotFound();
        }
    }
}