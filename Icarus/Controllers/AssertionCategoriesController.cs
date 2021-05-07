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
                int idac = db.tblAssertionCategories.Max(x => x.IDAssertionCategory);
                tblAssertionCategory assertioncat = new tblAssertionCategory();
                assertioncat.IDAssertionCategory = idac + 1;
                ViewData["Category"] = assertioncat;
                return View(db.tblAssertionCategories.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: AssertionCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Username"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblAssertionCategory tblAssertionCategory = db.tblAssertionCategories.Find(id);
                if (tblAssertionCategory == null)
                {
                    return HttpNotFound();
                }
                return View(tblAssertionCategory);
            }

            return RedirectToAction("Login", "Login");
        }

        // GET: AssertionCategories/Create
        public ActionResult Create()
        {
            if (Session["Username"] != null)
            {
                if (Session["isADG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y")
                {
                    return View();
                }
                return RedirectToAction("Index","AssertionCategories");
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: AssertionCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDAssertionCategory,Category")] tblAssertionCategory tblAssertionCategory)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    db.tblAssertionCategories.Add(tblAssertionCategory);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(tblAssertionCategory);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: AssertionCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Username"] != null) {
                if (Session["isADG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y")
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    tblAssertionCategory tblAssertionCategory = db.tblAssertionCategories.Find(id);
                    if (tblAssertionCategory == null)
                    {
                        return HttpNotFound();
                    }
                    return View(tblAssertionCategory);
                }
                return RedirectToAction("Index","AssertionCategories");
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: AssertionCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDAssertionCategory,Category")] tblAssertionCategory tblAssertionCategory)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    db.Entry(tblAssertionCategory).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(tblAssertionCategory);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: AssertionCategories/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (Session["Username"] != null) {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        tblAssertionCategory tblAssertionCategory = db.tblAssertionCategories.Find(id);
        //        if (tblAssertionCategory == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(tblAssertionCategory);
        //    }
        //    return RedirectToAction("Login", "Login");
        //}

        // POST: AssertionCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null) {
                tblAssertionCategory tblAssertionCategory = db.tblAssertionCategories.Find(id);
                db.tblAssertionCategories.Remove(tblAssertionCategory);
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
