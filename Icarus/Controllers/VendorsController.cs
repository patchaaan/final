using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Icarus.Models;

namespace Icarus.Controllers
{
    public class VendorsController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: tblVendors
        [Route("Vendors/")]
        public ActionResult Index()
        {
            if (Session["Username"] != null) {
                try
                {
                    int codepup = db.tblVendors.Max(x => x.IDVendor);
                    tblVendor vendor = new tblVendor();
                    vendor.IDVendor = codepup + 1;
                    ViewData["Vendor"] = vendor;
                    return View(db.tblVendors.ToList().OrderByDescending(x => x.IDVendor).ToList());
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }
            }
            return RedirectToAction("Login", "Login");

        }


        // POST: tblVendors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDVendor,Vendor,ContactPerson,ContactNumber,Email,Notes,IsActive,TIN")] tblVendor tblVendor)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.tblVendors.Add(tblVendor);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return View(tblVendor);
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public PartialViewResult EditPartial(int id)
        {
            tblVendor vendor = db.tblVendors.Find(id);
            return PartialView("_EditPartial", vendor);
        }

        // POST: tblVendors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDVendor,Vendor,ContactPerson,ContactNumber,Email,Notes,IsActive,TIN")] tblVendor tblVendor)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(tblVendor).State = EntityState.Modified;
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

        // POST: tblVendors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null) {
                try
                {
                    tblVendor tblVendor = db.tblVendors.Find(id);
                    db.tblVendors.Remove(tblVendor);
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
