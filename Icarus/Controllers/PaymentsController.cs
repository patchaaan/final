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
    public class PaymentsController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: Payments
        [Route("Payments/")]
        public ActionResult Index()
        {
            return View(db.vPaymentBrowses.ToList().OrderByDescending(p => p.IDPayment).ToList());
        }

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPayment tblPayment = db.tblPayments.Find(id);
            if (tblPayment == null)
            {
                return HttpNotFound();
            }
            return View(tblPayment);
        }

        // GET: Payments/Create
        public ActionResult Create()
        {
            var residents = db.tblResidents.Select(
                    s => new {
                        Text = s.Firstname + " '" + s.Nickname + "' " + s.Lastname,
                        Value = s.IDResident
                    }
                ).ToList();
            ViewBag.residentList = new SelectList(residents, "Value", "Text");
            ViewBag.paymentMethods = new SelectList(db.tblPaymentMethods, "IDPaymentMethod", "PaymentMethod");
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDPayment,PaidDate,IDAdmission,TotalPaid,IDPaymentMethod,Bank,CheckNo,CheckDate,Notes,IsVerified,PostedDate")] tblPayment tblPayment)
        {
            if (ModelState.IsValid)
            {
                db.tblPayments.Add(tblPayment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblPayment);
        }

        // GET: Payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPayment tblPayment = db.tblPayments.Find(id);
            if (tblPayment == null)
            {
                return HttpNotFound();
            }
            return View(tblPayment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDPayment,PaidDate,IDAdmission,TotalPaid,IDPaymentMethod,Bank,CheckNo,CheckDate,Notes,IsVerified,PostedDate")] tblPayment tblPayment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblPayment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblPayment);
        }

        // GET: Payments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPayment tblPayment = db.tblPayments.Find(id);
            if (tblPayment == null)
            {
                return HttpNotFound();
            }
            return View(tblPayment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblPayment tblPayment = db.tblPayments.Find(id);
            db.tblPayments.Remove(tblPayment);
            db.SaveChanges();
            return RedirectToAction("Index");
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
