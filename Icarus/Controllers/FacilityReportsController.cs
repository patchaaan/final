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
        public ActionResult Index()
        {
            return View(db.tblFacilityReports.ToList());
        }

        // GET: tblFacilityReports/Details/5
        public ActionResult Details(int? id)
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

        // GET: tblFacilityReports/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tblFacilityReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDFacilityReport,ReportDate,Activity,PostedBy")] tblFacilityReport tblFacilityReport)
        {
            if (ModelState.IsValid)
            {
                db.tblFacilityReports.Add(tblFacilityReport);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblFacilityReport);
        }

        // GET: tblFacilityReports/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: tblFacilityReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDFacilityReport,ReportDate,Activity,PostedBy")] tblFacilityReport tblFacilityReport)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblFacilityReport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblFacilityReport);
        }

        // GET: tblFacilityReports/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: tblFacilityReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblFacilityReport tblFacilityReport = db.tblFacilityReports.Find(id);
            db.tblFacilityReports.Remove(tblFacilityReport);
            db.SaveChanges();
            return RedirectToAction("Index");
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
