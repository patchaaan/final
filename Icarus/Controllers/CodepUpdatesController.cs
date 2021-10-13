using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
                try
                {
                    int codepup = db.tblCodepUpdates.Max(x => x.IDUpdate);
                    tblCodepUpdate codepupdate = new tblCodepUpdate();
                    codepupdate.IDAdmission = codepup + 1;
                    ViewData["CodepUpdate"] = codepupdate;
                    var residents = db.vAdmissionBrowses.Select(
                            s => new
                            {
                                Text = s.Resident,
                                Value = s.IDAdmission
                            }
                        ).ToList();
                    ViewBag.residentList = new SelectList(residents, "Value", "Text");
                    return View(db.vselCodepUpdateBrowses.ToList().OrderByDescending(x => x.IDUpdate).ToList());
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: CodepUpdates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDUpdate,DateUpdate,DateLog,UpdateType,UpdatedBy,UpdateSummary,IDAdmission")] tblCodepUpdate tblCodepUpdate)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        tblCodepUpdate.UpdatedBy = Session["Username"].ToString();
                        db.tblCodepUpdates.Add(tblCodepUpdate);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
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
                    try
                    {
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
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return RedirectToAction("Index","CodepUpdates");
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public PartialViewResult EditPartial(int id)
        {
            tblCodepUpdate update = new tblCodepUpdate();
            try
            {
                update = db.tblCodepUpdates.Find(id);
                var residents = db.vAdmissionBrowses.Select(
                                s => new
                                {
                                    Text = s.Resident,
                                    Value = s.IDAdmission
                                }
                            ).ToList();
                ViewBag.residentList = new SelectList(residents, "Value", "Text");
                return PartialView("_EditPartial", update);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception " + e);
            }
            return PartialView("_EditPartial", update);

        }

        // POST: CodepUpdates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDUpdate,DateUpdate,DateLog,UpdateType,UpdatedBy,UpdateSummary,IDAdmission")] tblCodepUpdate tblCodepUpdate)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(tblCodepUpdate).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return RedirectToAction("Index", "CodepUpdates");
            }
            return RedirectToAction("Login", "Login");

        }

        // POST: CodepUpdates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null)
            {
                try
                {
                    tblCodepUpdate codepUpdate = db.tblCodepUpdates.Find(id);
                    db.tblCodepUpdates.Remove(codepUpdate);
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
