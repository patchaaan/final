using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Icarus.Models;

namespace Icarus.Controllers
{
    public class InquiriesController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        [Route("Inquiries/")]
        // GET: Inquiries
        public ActionResult Index()
        {
            if (Session["Username"] != null)
            {
                try
                {
                    int inq = db.tblInquiries.Max(x => x.IDInquiry);
                    tblInquiry inquiry = new tblInquiry();
                    inquiry.IDInquiry = inq + 1;
                    ViewData["Inquiry"] = inquiry;
                    return View(db.tblInquiries.ToList().OrderByDescending(x => x.IDInquiry).ToList());
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }
                return HttpNotFound();
            }
            else {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // GET: Inquiries/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Username"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                try
                {
                    tblInquiry tblInquiry = db.tblInquiries.Find(id);
                    IEnumerable<tblInquiryCommLog> inqcommlog = db.tblInquiryCommLogs.ToList().Where(x => x.IDInquiry == tblInquiry.IDInquiry).ToList();
                    tblInquiryCommLog inquirylog = new tblInquiryCommLog();
                    inquirylog.IDInquiry = tblInquiry.IDInquiry;
                    ViewData["Inquiry"] = inquirylog;
                    ViewData["CommLogs"] = inqcommlog;
                    ViewBag.inqlog = true;

                    if (!inqcommlog.Any() || inqcommlog == null)
                    {
                        ViewBag.inqlog = false;
                    }

                    if (tblInquiry == null)
                    {
                        return HttpNotFound();
                    }
                    return View(tblInquiry);
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


        // POST: Inquiries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDInquiry,InquiryDate,Codep,ContactNo,Prospect,Details,FollowupOn,IDInquiryStatus")] tblInquiry tblInquiry)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.tblInquiries.Add(tblInquiry);
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

        [HttpPost, ActionName("CommLogCreate")]
        [ValidateAntiForgeryToken]
        public ActionResult CommLogCreate([Bind(Include = "IDInquiryCommLog,IDInquiry,CommDate,InitiatedBy,CommDetails")] tblInquiryCommLog commlog)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.tblInquiryCommLogs.Add(commlog);
                        db.SaveChanges();
                        return RedirectToAction("Details", new { id = commlog.IDInquiry });
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }

                return RedirectToAction("Details", new { id = commlog.IDInquiry });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: Inquiries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Username"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                try
                {
                    tblInquiry tblInquiry = db.tblInquiries.Find(id);
                    if (tblInquiry == null)
                    {
                        return HttpNotFound();
                    }
                    return View(tblInquiry);
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

        [HttpGet]
        public PartialViewResult EditPartial(int id)
        {
            tblInquiryCommLog commlog = db.tblInquiryCommLogs.Find(id);
            return PartialView("_EditPartial", commlog);
        }

        // POST: Inquiries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDInquiry,InquiryDate,Codep,ContactNo,Prospect,Details,FollowupOn,IDInquiryStatus")] tblInquiry tblInquiry)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(tblInquiry).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Details", new { id = tblInquiry.IDInquiry });
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return RedirectToAction("Details", new { id = tblInquiry.IDInquiry });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost, ActionName("CommLogsEdit")]
        [ValidateAntiForgeryToken]
        public ActionResult CommLogsEdit([Bind(Include = "IDInquiryCommLog,IDInquiry,CommDate,InitiatedBy,CommDetails")] tblInquiryCommLog commlog)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(commlog).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Details", new { id = commlog.IDInquiry });
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return RedirectToAction("Details", new { id = commlog.IDInquiry });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: Inquiries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null)
            {
                try
                {
                    tblInquiry tblInquiry = db.tblInquiries.Find(id);
                    db.tblInquiries.Remove(tblInquiry);
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

        [HttpPost, ActionName("DeleteCommLogs")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCommLogs(int id)
        {
            if (Session["Username"] != null)
            {
                int com_id = 0;
                try
                {
                    tblInquiryCommLog commlog = db.tblInquiryCommLogs.Find(id);
                    com_id = commlog.IDInquiry;
                    db.tblInquiryCommLogs.Remove(commlog);
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = commlog.IDInquiry });
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }
                return RedirectToAction("Details", new { id = com_id });
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
