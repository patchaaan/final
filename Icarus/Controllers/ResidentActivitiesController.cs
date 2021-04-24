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
    public class ResidentActivitiesController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: tblResidentActivities
        [Route("ResidentActivities/")]
        public ActionResult Index()
        {
            if (Session["Username"] != null) {
                ViewBag.ResidentLists = db.tblResidents.ToList();
                return View(db.tblResidentActivities.ToList().OrderByDescending(x => x.IDResidentActivityLog).ToList());
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: tblResidentActivities/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Username"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblResidentActivity tblResidentActivity = db.tblResidentActivities.Find(id);
                if (tblResidentActivity == null)
                {
                    return HttpNotFound();
                }
                return View(tblResidentActivity);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: tblResidentActivities/Create
        public ActionResult Create()
        {
            if (Session["Username"] != null) {
                var residents = db.tblResidents.Select(
                        s => new {
                            Text = s.Firstname + " '" + s.Nickname + "' " + s.Lastname,
                            Value = s.IDResident
                        }
                    ).ToList();
                ViewBag.residentList = new SelectList(residents, "Value", "Text");
                ViewBag.generatedBy = Session["Username"];
                ViewBag.ranks = new SelectList(db.tblRanks, "Rank", "Rank");
                return View();
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: tblResidentActivities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDResidentActivityLog,LogDate,IDAdmission,Position,Activity,PostedBy")] tblResidentActivity tblResidentActivity)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    tblResidentActivity.PostedBy = Session["Username"].ToString();
                    db.tblResidentActivities.Add(tblResidentActivity);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(tblResidentActivity);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: tblResidentActivities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Username"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblResidentActivity tblResidentActivity = db.tblResidentActivities.Find(id);
                if (tblResidentActivity == null)
                {
                    return HttpNotFound();
                }
                var residents = db.tblResidents.Select(
                        s => new
                        {
                            Text = s.Firstname + " '" + s.Nickname + "' " + s.Lastname,
                            Value = s.IDResident
                        }
                    ).ToList();
                ViewBag.residentList = new SelectList(residents, "Value", "Text");
                ViewBag.ranks = new SelectList(db.tblRanks, "Rank", "Rank");
                return View(tblResidentActivity);
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: tblResidentActivities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDResidentActivityLog,LogDate,IDAdmission,Position,Activity,PostedBy")] tblResidentActivity tblResidentActivity)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {

                    db.Entry(tblResidentActivity).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(tblResidentActivity);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: tblResidentActivities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Username"] != null) {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblResidentActivity tblResidentActivity = db.tblResidentActivities.Find(id);
                if (tblResidentActivity == null)
                {
                    return HttpNotFound();
                }
                return View(tblResidentActivity);
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: tblResidentActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null) {
                tblResidentActivity tblResidentActivity = db.tblResidentActivities.Find(id);
                db.tblResidentActivities.Remove(tblResidentActivity);
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
