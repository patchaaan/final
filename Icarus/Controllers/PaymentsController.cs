using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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
        [HttpGet]
        public ActionResult Index(DateTime? datefrom, DateTime? dateto)
        {
            var firstDay = new DateTime(DateTime.Now.Year, 1, 1);
            var secondDay = new DateTime(DateTime.Now.Year, 12, 31);
            if (Session["Username"] != null) {
                if (Session["isADG"].ToString() == "Y" || Session["isEDG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y") {
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
                    if (datefrom != null && dateto != null) {
                        ViewBag.clicked = true;
                        return View(db.vPaymentBrowses.Where(x => x.PaidDate >= datefrom && x.PaidDate <= dateto).ToList().OrderByDescending(p => p.IDPayment).ToList());
                    }
                    else if (datefrom != null && dateto == null) {
                        ViewBag.clicked = true;
                        return View(db.vPaymentBrowses.Where(x => x.PaidDate >= datefrom).ToList().OrderBy(p => p.IDPayment).ToList());
                    }
                    else {
                        ViewBag.clicked = false;
                        return View(db.vPaymentBrowses.Where(x => x.PaidDate >= firstDay && x.PaidDate <= secondDay).ToList().OrderByDescending(p => p.IDPayment).ToList());
                    }
                }
                return RedirectToAction("Index","Residents");
            }
            return RedirectToAction("Login", "Login");
        }

        //[HttpGet, ActionName("Verified")]
        //public ActionResult Verified(DateTime? datefrom, DateTime? dateto, String clicked)
        //{
        //    if (Session["Username"] != null)
        //    {
        //        if (Session["isADG"].ToString() == "Y" || Session["isEDG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y")
        //        {
        //            var residents = db.vAdmissionBrowses.Select(
        //                    s => new
        //                    {
        //                        Text = s.Resident,
        //                        Value = s.IDAdmission
        //                    }
        //                ).ToList();
        //            ViewBag.residentList = new SelectList(residents, "Value", "Text");
        //            ViewBag.paymentMethods = new SelectList(db.tblPaymentMethods, "IDPaymentMethod", "PaymentMethod");
        //            int pay = db.tblPayments.Max(x => x.IDPayment);
        //            tblPayment payment = new tblPayment();
        //            payment.IDPayment = pay + 1;
        //            ViewData["Payment"] = payment;
        //            //ViewBag.datefrom = @Convert.ToDateTime(datefrom).ToString("dd/MM/yyyy");
        //            //ViewBag.dateto = @Convert.ToDateTime(dateto).ToString("dd/MM/yyyy");
        //            var firstDay = new DateTime(DateTime.Now.Year, 1, 1);
        //            var secondDay = new DateTime(DateTime.Now.Year, 12, 31);

        //            if (datefrom != null && dateto != datefrom) {
        //                ViewBag.datefrom = @Convert.ToDateTime(datefrom).ToString("yyyy-MM-dd");
        //                ViewBag.dateto = @Convert.ToDateTime(dateto).ToString("yyyy-MM-dd");
        //                if (clicked == "F")
        //                {
        //                    ViewBag.clicked = "T";
        //                    return View("Index", db.vPaymentBrowses.Where(y => y.PaidDate >= datefrom && y.PaidDate <= dateto).ToList().OrderByDescending(y => y.IsVerified).ToList());
        //                }
        //                else
        //                {
        //                    ViewBag.clicked = "F";
        //                    return View("Index", db.vPaymentBrowses.Where(y => y.PaidDate >= datefrom && y.PaidDate <= dateto).ToList());
        //                }
        //            } else if (datefrom != null && dateto == datefrom) {
        //                ViewBag.datefrom = @Convert.ToDateTime(datefrom).ToString("yyyy-MM-dd");
        //                ViewBag.dateto = @Convert.ToDateTime(datefrom).ToString("yyyy-MM-dd");
        //                if (clicked == "F")
        //                {
        //                    ViewBag.clicked = "T";
        //                    return View("Index", db.vPaymentBrowses.Where(y => y.PaidDate >= datefrom).ToList().OrderByDescending(y => y.IsVerified).ToList());
        //                }
        //                else
        //                {
        //                    ViewBag.clicked = "F";
        //                    return View("Index", db.vPaymentBrowses.Where(y => y.PaidDate >= datefrom).ToList().OrderByDescending(x => x.IDPayment).ToList());
        //                }
        //            } else {
        //                ViewBag.datefrom = @Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy-MM-dd"); ;
        //                ViewBag.dateto = @Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy-MM-dd"); ;
        //                if (clicked == "F") {
        //                    ViewBag.clicked = "T";
        //                    return View("Index", db.vPaymentBrowses.Where(y => y.PaidDate >= firstDay && y.PaidDate <= secondDay).ToList().OrderByDescending(y => y.IsVerified).ToList());
        //                }
        //                else {
        //                    ViewBag.clicked = "F";
        //                    return View("Index", db.vPaymentBrowses.Where(y => y.PaidDate >= firstDay && y.PaidDate <= secondDay).ToList().OrderByDescending(y => y.IDPayment).ToList());
        //                }
        //            }
        //        }
        //        return RedirectToAction("Index", "Residents");
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //}

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Username"] != null) {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
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
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public PartialViewResult DetailsPartial(int id)
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
            tblPayment payments = db.tblPayments.Find(id);
            return PartialView("_DetailsPartial", payments);
        }

        // GET: Payments/Create
        public ActionResult Create()
        {
            if (Session["Username"] != null) {
                if (Session["isADG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y")
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
                    return View();
                }
                return RedirectToAction("Index", "Residents");
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDPayment,PaidDate,IDAdmission,TotalPaid,IDPaymentMethod,Bank,CheckNo,CheckDate,Notes,IsVerified,PostedDate")] tblPayment tblPayment)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    //tblAdmission tblAdmission = db.tblAdmissions.Where(x => x.IDResident == tblPayment.IDAdmission).FirstOrDefault();
                    //tblPayment.IDAdmission = tblAdmission.IDAdmission;
                    db.tblPayments.Add(tblPayment);
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("[dbo].[spRecalcAdmissionBalance] @IDAdmission", new SqlParameter("IDAdmission", tblPayment.IDAdmission));
                    return RedirectToAction("Index");
                }

                return View(tblPayment);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: Payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Username"] != null) {
                if (Session["isADG"].ToString() == "Y")
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDPayment,PaidDate,IDAdmission,TotalPaid,IDPaymentMethod,Bank,CheckNo,CheckDate,Notes,IsVerified,PostedDate")] tblPayment tblPayment)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    db.Entry(tblPayment).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(tblPayment);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: Payments/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (Session["Username"] != null) {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        tblPayment tblPayment = db.tblPayments.Find(id);
        //        if (tblPayment == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(tblPayment);
        //    }
        //    return RedirectToAction("Login", "Login");
        //}

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null) {
                tblPayment tblPayment = db.tblPayments.Find(id);
                db.tblPayments.Remove(tblPayment);
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
