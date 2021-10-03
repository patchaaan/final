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
    public class StaffsController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: Staffs
        [Route("Staffs/")]
        public ActionResult Index()
        {
            if (Session["Username"] != null)
            {
                int staff = db.tblStaffs.Max(x => x.IDStaff);
                tblStaff modelstaff = new tblStaff();
                modelstaff.IDStaff = staff + 1;
                ViewData["Staff"] = modelstaff;
                return View(db.tblStaffs.ToList().OrderBy(p => p.Lastname).ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: Staffs/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Username"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblStaff tblStaff = db.tblStaffs.Find(id);
                if (tblStaff == null)
                {
                    return HttpNotFound();
                }
                return View(tblStaff);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // GET: Staffs/Create
        public ActionResult Create()
        {
            if (Session["Username"] != null)
            {
                if (Session["isADG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y")
                {
                    return View();
                }
                return RedirectToAction("Index","Staffs");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "IDStaff,Lastname,Firstname,ContactNo,DateHired,DateTerminated,Status,Notes,Username,Email,Password,isADG,isEDG,isPG,isAAG")] tblStaff tblStaff)
        {
                if (ModelState.IsValid)
                {
                    var checkAcount = db.tblStaffs.Where(x => x.Username.Contains(tblStaff.Username)).FirstOrDefault();
                    if (tblStaff.Password.Length < 8) {
                        return Json("Password");
                    }
                    if (checkAcount != null)
                    {
                        return Json(false);
                    }
                    else {
                        db.tblStaffs.Add(tblStaff);
                        db.SaveChanges();
                        return Json(true);
                    }
                }
                return Json(tblStaff);
            
        }

        // GET: Staffs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Username"] != null)
            {
                if (Session["isADG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y")
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    tblStaff tblStaff = db.tblStaffs.Find(id);
                    if (tblStaff == null)
                    {
                        return HttpNotFound();
                    }
                    return View(tblStaff);
                }
                return RedirectToAction("Index","Staffs");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDStaff,Lastname,Firstname,ContactNo,DateHired,DateTerminated,Status,Notes,Username,Email,Password,isADG,isEDG,isPG,isAAG")] tblStaff tblStaff)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
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
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }


        //[HttpPost, ActionName("EditUsername")]
        //public JsonResult EditUsername()
        //{
        //    return JsonResult = null;
        //}

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null)
            {
                tblStaff tblStaff = db.tblStaffs.Find(id);
                db.tblStaffs.Remove(tblStaff);
                db.SaveChanges();
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
