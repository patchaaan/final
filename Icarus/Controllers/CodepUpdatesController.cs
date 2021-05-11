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
    public class CodepUpdatesController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: CodepUpdates
        [Route("CodepUpdates/")]
        public ActionResult Index()
        {
            if (Session["Username"] != null) {
                int codepup = db.tblCodepUpdates.Max(x => x.IDUpdate);
                tblCodepUpdate codepupdate = new tblCodepUpdate();
                codepupdate.IDAdmission = codepup + 1;
                ViewData["CodepUpdate"] = codepupdate;
                var residents = db.vAdmissionBrowses.Select(
                        s => new {
                            Text = s.Resident,
                            Value = s.IDAdmission
                        }
                    ).ToList();
                ViewBag.residentList = new SelectList(residents, "Value", "Text");
                return View(db.vselCodepUpdateBrowses.ToList().OrderByDescending(x => x.IDUpdate).ToList());
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: CodepUpdates/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Username"] != null) {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblCodepUpdate codepUpdate = db.tblCodepUpdates.Find(id);
                if (codepUpdate == null)
                {
                    return HttpNotFound();
                }
                return View(codepUpdate);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: CodepUpdates/Create
        public ActionResult Create()
        {
            if (Session["Username"] != null)
            {
                if(Session["isADG"].ToString() == "Y" || Session["isEDG"].ToString() == "Y" || Session["isPG"].ToString() == "Y")
                {
                    var residents = db.vAdmissionBrowses.Select(
                        s => new {
                            Text = s.Resident,
                            Value = s.IDAdmission
                        }
                    ).ToList();
                    ViewBag.residentList = new SelectList(residents, "Value", "Text");
                    return View();
                }
                return RedirectToAction("Index","CodepUpdates");
            }
            return RedirectToAction("Login", "Login");

        }

        // POST: CodepUpdates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDUpdate,DateUpdate,DateLog,UpdateType,UpdatedBy,UpdateSummary,IDAdmission")] tblCodepUpdate tblCodepUpdate)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    tblCodepUpdate.UpdatedBy = Session["Username"].ToString();
                    db.tblCodepUpdates.Add(tblCodepUpdate);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index", "CodepUpdates");

            }
            return RedirectToAction("Login", "Login");
        }

        // GET: CodepUpdates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Username"] != null)
            {
                if (Session["isADG"].ToString() == "Y" || Session["isEDG"].ToString() == "Y" || Session["isPG"].ToString() == "Y")
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    tblCodepUpdate codepUpdate = db.tblCodepUpdates.Find(id);
                    if (codepUpdate == null)
                    {
                        return HttpNotFound();
                    }
                    var residents = db.vAdmissionBrowses.Select(
                            s => new
                            {
                                Text = s.Resident,
                                Value = s.IDAdmission
                            }
                        ).ToList();
                    ViewBag.residentList = new SelectList(residents, "Value", "Text");
                    return View(codepUpdate);
                }
                return RedirectToAction("Index","CodepUpdates");
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public PartialViewResult EditPartial(int id)
        {
            tblPayment payment = db.tblPayments.Find(id);
            var residents = db.vAdmissionBrowses.Select(
                            s => new
                            {
                                Text = s.Resident,
                                Value = s.IDAdmission
                            }
                        ).ToList();
            ViewBag.residentList = new SelectList(residents, "Value", "Text");
            return PartialView("_EditPartial", payment);
        }

        // POST: CodepUpdates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDUpdate,DateUpdate,DateLog,UpdateType,UpdatedBy,UpdateSummary,IDAdmission")] tblCodepUpdate tblCodepUpdate)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(tblCodepUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index", "CodepUpdates");
            }
            return RedirectToAction("Login", "Login");

        }

        // GET: CodepUpdates/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (Session["Username"] != null)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        vselCodepUpdateBrowse vselCodepUpdateBrowse = db.vselCodepUpdateBrowses.Find(id);
        //        if (vselCodepUpdateBrowse == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(vselCodepUpdateBrowse);
        //    }
        //    return RedirectToAction("Login", "Login");

        //}

        // POST: CodepUpdates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null)
            {
                tblCodepUpdate codepUpdate = db.tblCodepUpdates.Find(id);
                db.tblCodepUpdates.Remove(codepUpdate);
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
