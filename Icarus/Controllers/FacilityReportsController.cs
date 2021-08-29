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
    public class FacilityReportsController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: tblFacilityReports
        [Route("FacilityReports/")]
        public ActionResult Index()
        {
            if (Session["Username"] != null) {
                var fac = db.tblFacilityReports.ToList().OrderByDescending(x => x.IDFacilityReport).FirstOrDefault();
                if (fac == null)
                {
                    tblFacilityReport facility = new tblFacilityReport();
                    facility.IDFacilityReport = 1;
                    ViewData["FacReport"] = facility;
                    return View(db.tblFacilityReports.ToList().OrderByDescending(x => x.IDFacilityReport).OrderByDescending(x => x.ReportDate).ToList());
                }
                else {
                    tblFacilityReport facility = new tblFacilityReport();
                    facility.IDFacilityReport = fac.IDFacilityReport + 1;
                    ViewData["FacReport"] = facility;
                    return View(db.tblFacilityReports.ToList().OrderByDescending(x => x.IDFacilityReport).OrderByDescending(x => x.ReportDate).ToList());
                }
                
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: tblFacilityReports/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Username"] != null) {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblFacilityReport tblFacilityReport = db.tblFacilityReports.Find(id);
                if (tblFacilityReport == null)
                {
                    return HttpNotFound();
                }
                return View(tblFacilityReport);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: tblFacilityReports/Create
        public ActionResult Create()
        {
            if (Session["Username"] != null) {
                if (Session["isADG"].ToString() == "Y" || Session["isPG"].ToString() == "Y")
                {
                    return View();
                }
                return RedirectToAction("Index", "FacilityReports");
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: tblFacilityReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDFacilityReport,ReportDate,Activity,PostedBy")] tblFacilityReport tblFacilityReport)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    tblFacilityReport.PostedBy = Session["Username"].ToString();
                    db.tblFacilityReports.Add(tblFacilityReport);
                    db.SaveChanges();
                    return RedirectToAction("Index", "FacilityReports");
                }
                return View(tblFacilityReport);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: tblFacilityReports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Username"] != null) {
                if (Session["isADG"].ToString() == "Y" || Session["isPG"].ToString() == "Y")
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    tblFacilityReport tblFacilityReport = db.tblFacilityReports.Find(id);
                    if (tblFacilityReport == null)
                    {
                        return HttpNotFound();
                    }
                    return View(tblFacilityReport);
                }
                return RedirectToAction("Index", "FacilityReports");
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public PartialViewResult EditPartial(int id)
        {
            tblFacilityReport facilityreport = db.tblFacilityReports.Find(id);
            return PartialView("_EditPartial", facilityreport);
        }

        // POST: tblFacilityReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDFacilityReport,ReportDate,Activity,PostedBy")] tblFacilityReport tblFacilityReport)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    db.Entry(tblFacilityReport).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(tblFacilityReport);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: tblFacilityReports/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (Session["Username"] != null) {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        tblFacilityReport tblFacilityReport = db.tblFacilityReports.Find(id);
        //        if (tblFacilityReport == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(tblFacilityReport);
        //    }
        //    return RedirectToAction("Login", "Login");
        //}

        // POST: tblFacilityReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null) {
                tblFacilityReport tblFacilityReport = db.tblFacilityReports.Find(id);
                db.tblFacilityReports.Remove(tblFacilityReport);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Login", "Login");
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
