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
    public class VendorsController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: tblVendors
        [Route("Vendors/")]
        public ActionResult Index()
        {
            if (Session["Username"] != null) {
                return View(db.tblVendors.ToList().OrderByDescending(x => x.IDVendor).ToList());
            }
            return RedirectToAction("Login", "Login");

        }

        // GET: tblVendors/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Username"] != null) {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblVendor tblVendor = db.tblVendors.Find(id);
                if (tblVendor == null)
                {
                    return HttpNotFound();
                }
                return View(tblVendor);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: tblVendors/Create
        public ActionResult Create()
        {
            if (Session["Username"] != null) {
                if (Session["isADG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y")
                {
                    return View();
                }
                return RedirectToAction("Index", "Vendors");
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: tblVendors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDVendor,Vendor,ContactPerson,ContactNumber,Email,Notes,IsActive,TIN")] tblVendor tblVendor)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    db.tblVendors.Add(tblVendor);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(tblVendor);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: tblVendors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Username"] != null) {
                if (Session["isADG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y")
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    tblVendor tblVendor = db.tblVendors.Find(id);
                    if (tblVendor == null)
                    {
                        return HttpNotFound();
                    }
                    return View(tblVendor);
                }
                return RedirectToAction("Index", "Vendors");
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: tblVendors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDVendor,Vendor,ContactPerson,ContactNumber,Email,Notes,IsActive,TIN")] tblVendor tblVendor)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    db.Entry(tblVendor).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(tblVendor);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: tblVendors/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (Session["Username"] != null) {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        tblVendor tblVendor = db.tblVendors.Find(id);
        //        if (tblVendor == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(tblVendor);
        //    }
        //    return RedirectToAction("Login", "Login");
        //}

        // POST: tblVendors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null) {
                tblVendor tblVendor = db.tblVendors.Find(id);
                db.tblVendors.Remove(tblVendor);
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
