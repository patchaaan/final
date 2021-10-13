using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Icarus.Models;

namespace Icarus.Controllers
{
    public class RequestsController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        [Route("Requests/")]
        public ActionResult Index()
        {
            if (Session["Username"] != null)
            {
                try
                {
                    IEnumerable<tblRequestStatu> tblRequestStatus = db.tblRequestStatus.ToList();
                    ViewData["requestStatus"] = tblRequestStatus;
                    int req = db.tblRequests.Max(x => x.IDRequest);
                    ViewBag.requests = new SelectList(db.tblRequestStatus, "IDRequestStatus", "Status");
                    tblRequest request = new tblRequest();
                    request.IDRequest = req + 1;
                    ViewData["Request"] = request;
                    return View(db.tblRequests.ToList().OrderByDescending(x => x.IDRequest).ToList());
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }
            }
            else {
                return RedirectToAction("Login", "Login");
            }
            return HttpNotFound();
        }

        // POST: Requests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDRequest,DateRequest,DateNeeded,RequestedBy,Request,Budget,ApprovedBy,IDRequestStatus,ApproverNotes,DateApproved,RequestorEmail,DateAcc,RequestorNotes")] tblRequest tblRequest)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.tblRequests.Add(tblRequest);
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
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpGet]
        public PartialViewResult EditPartial(int id)
        {
            tblRequest request = db.tblRequests.Find(id);
            request.DateRequest = request.DateRequest.Date;
            ViewBag.requests = new SelectList(db.tblRequestStatus, "IDRequestStatus", "Status");
            return PartialView("_EditPartial", request);
        }

        // POST: Requests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDRequest,DateRequest,DateNeeded,RequestedBy,Request,Budget,ApprovedBy,IDRequestStatus,ApproverNotes,DateApproved,RequestorEmail,DateAcc,RequestorNotes")] tblRequest tblRequest)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(tblRequest).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return View(tblRequest);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null)
            {
                try
                {
                    tblRequest tblRequest = db.tblRequests.Find(id);
                    db.tblRequests.Remove(tblRequest);
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
