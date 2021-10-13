using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Icarus.Models;

namespace Icarus.Controllers
{
    public class ResidentsController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        [Route("Residents/")]
        // GET: Residents
        public ActionResult Index()
        {
            if (Session["Username"] != null)
            {
                try
                {
                    int res = db.tblResidents.Max(x => x.IDResident);
                    tblResident resident = new tblResident();
                    resident.IDResident = res + 1;
                    ViewData["Resident"] = resident;
                    return View(db.tblResidents.ToList().OrderByDescending(p => p.IDResident).ToList());
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

        [HttpGet]
        public PartialViewResult DetailsPartial(int id)
        {
            tblResident resident = db.tblResidents.Find(id);
            return PartialView("_DetailsPartial", resident);
        }


        // POST: Residents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDResident,Lastname,Firstname,Middlename,Nickname,Birthdate,Age,Sex,Codep,Relationship,ContactNumber,EmailAddress")] tblResident tblResident)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.tblResidents.Add(tblResident);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return View(tblResident);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpGet]
        public PartialViewResult EditPartial(int id)
        {
            tblResident resident = db.tblResidents.Find(id);
            return PartialView("_EditPartial", resident);
        }

        // POST: Residents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDResident,Lastname,Firstname,Middlename,Nickname,Birthdate,Age,Sex,Codep,Relationship,ContactNumber,EmailAddress")] tblResident tblResident)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(tblResident).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return View(tblResident);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // POST: Residents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null)
            {
                try
                {
                    tblResident tblResident = db.tblResidents.Find(id);
                    db.tblResidents.Remove(tblResident);
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
