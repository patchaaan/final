using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Icarus.Models;

namespace Icarus.Controllers
{
    public class PaymentsController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: Payments
        [Route("Payments/")]
        [HttpGet]
        public ActionResult Index(DateTime? datefrom, DateTime? dateto)
        {
            try
            {
                var firstDay = new DateTime(DateTime.Now.Year, 1, 1);
                var secondDay = new DateTime(DateTime.Now.Year, 12, 31);
                if (Session["Username"] != null)
                {
                    if (Session["isADG"].ToString() == "Y" || Session["isEDG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y")
                    {
                        var residents = db.vAdmissionBrowses.Select(
                            s => new
                            {
                                Text = s.Resident,
                                Value = s.IDAdmission
                            }
                        ).ToList();
                        ViewBag.residentList = new SelectList(residents, "Value", "Text");
                        ViewBag.paymentMethods = new SelectList(db.tblPaymentMethods, "IDPaymentMethod", "PaymentMethod");
                        int pay = db.tblPayments.Max(x => x.IDPayment);
                        tblPayment payment = new tblPayment();
                        payment.IDPayment = pay + 1;
                        ViewData["Payment"] = payment;
                        ViewBag.totalUnverified = db.tblPayments.Where(x => x.IsVerified == "N").ToList().Count();
                        var totalpaid = db.tblPayments.Where(x => x.IsVerified == "N").Select(y => y.TotalPaid).ToList().Sum();
                        ViewBag.totalPaidUnverified = Math.Round((Double)totalpaid, 2);
                        if (datefrom != null && dateto != null)
                        {
                            ViewBag.clicked = true;
                            return View(db.vPaymentBrowses.Where(x => x.PaidDate >= datefrom && x.PaidDate <= dateto).ToList().OrderByDescending(p => p.IDPayment).ToList());
                        }
                        else if (datefrom != null && dateto == null)
                        {
                            ViewBag.clicked = true;
                            return View(db.vPaymentBrowses.Where(x => x.PaidDate >= datefrom).ToList().OrderBy(p => p.IDPayment).ToList());
                        }
                        else
                        {
                            ViewBag.clicked = false;
                            return View(db.vPaymentBrowses.Where(x => x.PaidDate >= firstDay && x.PaidDate <= secondDay).ToList().OrderByDescending(p => p.IDPayment).ToList());
                        }
                    }
                    return RedirectToAction("Index", "Residents");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception " + e);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Username"] != null) {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                try
                {
                    tblPayment tblPayment = db.tblPayments.Find(id);
                    tblAdmission admission = db.tblAdmissions.Find(tblPayment.IDAdmission);
                    tblResident resident = db.tblResidents.Find(admission.IDResident);
                    IEnumerable<tblPaymentMethod> paymentMethod = db.tblPaymentMethods.ToList();

                    ViewData["method"] = paymentMethod;
                    ViewBag.residentName = resident.Firstname + ' ' + resident.Nickname + ' ' + resident.Lastname;
                    if (tblPayment == null)
                    {
                        return HttpNotFound();
                    }
                    return View(tblPayment);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public PartialViewResult DetailsPartial(int id)
        {
            tblPayment payments = new tblPayment();
            try
            {
                var residents = db.vAdmissionBrowses.Select(
                            s => new
                            {
                                Text = s.Resident,
                                Value = s.IDAdmission
                            }
                        ).ToList();
                ViewBag.residentList = new SelectList(residents, "Value", "Text");
                ViewBag.paymentMethods = new SelectList(db.tblPaymentMethods, "IDPaymentMethod", "PaymentMethod");
                payments = db.tblPayments.Find(id);
                return PartialView("_DetailsPartial", payments);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception " + e);
            }
            return PartialView("_DetailsPartial", payments);
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDPayment,PaidDate,IDAdmission,TotalPaid,IDPaymentMethod,Bank,CheckNo,CheckDate,Notes,IsVerified,PostedDate")] tblPayment tblPayment)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.tblPayments.Add(tblPayment);
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("[dbo].[spRecalcAdmissionBalance] @IDAdmission", new SqlParameter("IDAdmission", tblPayment.IDAdmission));
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

        // GET: Payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Username"] != null) {
                if (Session["isADG"].ToString() == "Y")
                {
                    try
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        tblPayment tblPayment = db.tblPayments.Find(id);
                        tblAdmission admission = db.tblAdmissions.Find(tblPayment.IDAdmission);
                        tblResident resident = db.tblResidents.Find(admission.IDResident);
                        ViewBag.residentName = resident.Firstname + ' ' + resident.Nickname + ' ' + resident.Lastname;

                        ViewBag.paymentMethod = new SelectList(db.tblPaymentMethods, "IDPaymentMethod", "PaymentMethod");
                        if (tblPayment == null)
                        {
                            return HttpNotFound();
                        }
                        return View(tblPayment);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return RedirectToAction("Index","Residents");
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
            ViewBag.paymentMethod = new SelectList(db.tblPaymentMethods, "IDPaymentMethod", "PaymentMethod");
            return PartialView("_EditPartial", payment);
        }

        // POST: Payments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDPayment,PaidDate,IDAdmission,TotalPaid,IDPaymentMethod,Bank,CheckNo,CheckDate,Notes,IsVerified,PostedDate")] tblPayment tblPayment)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(tblPayment).State = EntityState.Modified;
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("[dbo].[spRecalcAdmissionBalance] @IDAdmission", new SqlParameter("IDAdmission", tblPayment.IDAdmission));
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }
                return View(tblPayment);
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null) {
                try
                {
                    tblPayment tblPayment = db.tblPayments.Find(id);
                    db.tblPayments.Remove(tblPayment);
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
