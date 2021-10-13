using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
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
                try
                {
                    var fac = db.tblFacilityReports.ToList().OrderByDescending(x => x.IDFacilityReport).FirstOrDefault();
                    if (fac == null)
                    {
                        tblFacilityReport facility = new tblFacilityReport();
                        facility.IDFacilityReport = 1;
                        ViewData["FacReport"] = facility;
                        return View(db.tblFacilityReports.ToList().OrderByDescending(x => x.IDFacilityReport).OrderByDescending(x => x.ReportDate).ToList());
                    }
                    else
                    {
                        tblFacilityReport facility = new tblFacilityReport();
                        facility.IDFacilityReport = fac.IDFacilityReport + 1;
                        ViewData["FacReport"] = facility;
                        return View(db.tblFacilityReports.ToList().OrderByDescending(x => x.IDFacilityReport).OrderByDescending(x => x.ReportDate).ToList());
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }

            }
            return RedirectToAction("Login", "Login");
        }

        // POST: tblFacilityReports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDFacilityReport,ReportDate,Activity,PostedBy")] tblFacilityReport tblFacilityReport)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    try
                    {
                        tblFacilityReport.PostedBy = Session["Username"].ToString();
                        db.tblFacilityReports.Add(tblFacilityReport);
                        db.SaveChanges();
                        return RedirectToAction("Index", "FacilityReports");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return View(tblFacilityReport);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDFacilityReport,ReportDate,Activity,PostedBy")] tblFacilityReport tblFacilityReport)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(tblFacilityReport).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return View(tblFacilityReport);
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: tblFacilityReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null) {
                try
                {
                    tblFacilityReport tblFacilityReport = db.tblFacilityReports.Find(id);
                    db.tblFacilityReports.Remove(tblFacilityReport);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }
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
