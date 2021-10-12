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
    public class AssertionCategoriesController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        [Route("AssertionCategories/")]
        // GET: AssertionCategories
        public ActionResult Index()
        {
            if (Session["Username"] != null)
            {
                try
                {
                    int idac = db.tblAssertionCategories.Max(x => x.IDAssertionCategory);
                    tblAssertionCategory assertioncat = new tblAssertionCategory();
                    assertioncat.IDAssertionCategory = idac + 1;
                    ViewData["Category"] = assertioncat;
                    return View(db.tblAssertionCategories.ToList());
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

        // POST: AssertionCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDAssertionCategory,Category")] tblAssertionCategory tblAssertionCategory)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.tblAssertionCategories.Add(tblAssertionCategory);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }

                return View(tblAssertionCategory);
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public PartialViewResult EditPartial(int id)
        {
            tblAssertionCategory tblAssertionCategory = new tblAssertionCategory();
            try
            {
                tblAssertionCategory = db.tblAssertionCategories.Find(id);
                return PartialView("_EditPartial", tblAssertionCategory);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception " + e);
            }
            return PartialView("_EditPartial", tblAssertionCategory);
        }

        // POST: AssertionCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDAssertionCategory,Category")] tblAssertionCategory tblAssertionCategory)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(tblAssertionCategory).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return View(tblAssertionCategory);
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: AssertionCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null) {
                try
                {
                    tblAssertionCategory tblAssertionCategory = db.tblAssertionCategories.Find(id);
                    db.tblAssertionCategories.Remove(tblAssertionCategory);
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
