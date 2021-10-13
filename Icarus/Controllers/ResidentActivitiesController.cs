using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
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
                try
                {
                    var residents = db.vAdmissionBrowses.Select(
                            s => new
                            {
                                Text = s.Resident,
                                Value = s.IDAdmission
                            }
                        ).ToList();
                    ViewBag.residents = db.vAdmissionBrowses.ToList();
                    ViewBag.residentList = new SelectList(residents, "Value", "Text");
                    ViewBag.generatedBy = Session["Username"];
                    ViewBag.ranks = new SelectList(db.tblRanks, "Rank", "Rank");
                    ViewData["ActivitiesList"] = db.tblResidentActivities.ToList().OrderByDescending(x => x.IDResidentActivityLog).ToList();

                    var resact = db.tblResidentActivities.ToList().OrderByDescending(x => x.IDResidentActivityLog).FirstOrDefault();
                    if (resact == null)
                    {
                        tblResidentActivity res = new tblResidentActivity();
                        res.IDResidentActivityLog = 1;
                        return View(res);
                    }
                    else
                    {
                        tblResidentActivity residentact = new tblResidentActivity();
                        residentact.IDResidentActivityLog = resact.IDResidentActivityLog + 1;
                        return View(residentact);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: tblResidentActivities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDResidentActivityLog,LogDate,IDAdmission,Position,Activity,PostedBy")] tblResidentActivity tblResidentActivity)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    try
                    {
                        tblResidentActivity.PostedBy = Session["Username"].ToString();
                        db.tblResidentActivities.Add(tblResidentActivity);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public PartialViewResult EditPartial(int id)
        {
            tblResidentActivity activity = db.tblResidentActivities.Find(id);
            var residents = db.vAdmissionBrowses.Select(
                            s => new
                            {
                                Text = s.Resident,
                                Value = s.IDAdmission
                            }
                        ).ToList();
            ViewBag.residentList = new SelectList(residents, "Value", "Text");
            ViewBag.ranks = new SelectList(db.tblRanks, "Rank", "Rank");
            return PartialView("_EditPartial", activity);
        }

        // POST: tblResidentActivities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDResidentActivityLog,LogDate,IDAdmission,Position,Activity,PostedBy")] tblResidentActivity tblResidentActivity)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(tblResidentActivity).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
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
                try
                {
                    tblResidentActivity tblResidentActivity = db.tblResidentActivities.Find(id);
                    db.tblResidentActivities.Remove(tblResidentActivity);
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
