using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Icarus.Models;

namespace Icarus.Controllers
{
    public class StaffsController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: Staffs
        [Route("Staffs/")]
        public ActionResult Index()
        {
            if (Session["Username"] != null)
            {
                try
                {
                    int staff = db.tblStaffs.Max(x => x.IDStaff);
                    tblStaff modelstaff = new tblStaff();
                    modelstaff.IDStaff = staff + 1;
                    ViewData["Staff"] = modelstaff;
                    return View(db.tblStaffs.ToList().OrderBy(p => p.Lastname).ToList());
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }
                return HttpNotFound();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: Staffs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "IDStaff,Lastname,Firstname,ContactNo,DateHired,DateTerminated,Status,Notes,Username,Email,Password,isADG,isEDG,isPG,isAAG")] tblStaff tblStaff)
        {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var checkAcount = db.tblStaffs.Where(x => x.Username.Contains(tblStaff.Username)).FirstOrDefault();
                        if (tblStaff.Password.Length < 8)
                        {
                            return Json("Password");
                        }
                        if (checkAcount != null)
                        {
                            return Json(false);
                        }
                        else
                        {
                            db.tblStaffs.Add(tblStaff);
                            db.SaveChanges();
                            return Json(true);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return Json(tblStaff);
            
        }

        [HttpGet]

        public PartialViewResult EditPartial(int id)
        {
            tblStaff staff = db.tblStaffs.Find(id);
            return PartialView("_EditPartial", staff);
        }

        [HttpGet]
        public PartialViewResult DetailsPartial(int id)
        {
            tblStaff staff = db.tblStaffs.Find(id);
            return PartialView("_DetailsPartial", staff);
        }

        // POST: Staffs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDStaff,Lastname,Firstname,ContactNo,DateHired,DateTerminated,Status,Notes,Username,Email,Password,isADG,isEDG,isPG,isAAG")] tblStaff tblStaff)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var checkAcount = db.tblStaffs.AsNoTracking().Where(x => x.Username == tblStaff.Username).FirstOrDefault();
                        if (tblStaff.Password.Length < 8)
                        {
                            return Json("Password");
                        }
                        else if (checkAcount != null && tblStaff.IDStaff != checkAcount.IDStaff)
                        {
                            return Json(false);
                        }
                        else
                        {
                            db.Entry(tblStaff).State = EntityState.Modified;
                            db.SaveChanges();
                            if (tblStaff.IDStaff.ToString() == Session["ID"].ToString())
                            {
                                Session["isAAG"] = tblStaff.isAAG.ToString();
                                Session["isADG"] = tblStaff.isADG.ToString();
                                Session["isEDG"] = tblStaff.isEDG.ToString();
                                Session["isPG"] = tblStaff.isPG.ToString();
                                Session["Username"] = tblStaff.Username.ToString();
                                Session["Password"] = tblStaff.Password.ToString();
                                Session["ID"] = tblStaff.IDStaff.ToString();
                            }
                            return Json(true);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null)
            {
                try
                {
                    tblStaff tblStaff = db.tblStaffs.Find(id);
                    db.tblStaffs.Remove(tblStaff);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
