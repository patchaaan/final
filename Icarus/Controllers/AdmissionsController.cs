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
    public class AdmissionsController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        [Route("Admissions/")]
        // GET: Admissions
        public ActionResult Index()
        {
            if (Session["Username"] != null)
            {
                return View(db.vAdmissionBrowses.ToList().OrderByDescending(p => p.IDAdmission).ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: Admissions/Details/5
        public ActionResult Details(int? id)
        {
            if(Session["Username"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                tblAdmission admission = db.tblAdmissions.SingleOrDefault(x => x.IDAdmission == id);
                tblRank Rank = db.tblRanks.Where(x => x.IDRank == admission.IDRank).SingleOrDefault();
                IEnumerable<tblAdmissionBilling> admissionBilling = db.tblAdmissionBillings.ToList().Where(x => x.IDAdmission == id).OrderByDescending(y => y.IDAdmissionBilling).ToList();
                IEnumerable<tblAssertion> assertion = db.tblAssertions.ToList().Where(x => x.IDAdmission == id).OrderByDescending(y => y.IDAssertion).ToList();
                IEnumerable<tblAdmissionVitalSign> vitalSigns = db.tblAdmissionVitalSigns.ToList().Where(x => x.IDAdmission == id).OrderByDescending(y => y.IDVitalSign).ToList();
                IEnumerable<tblAdmissionCommLog> commLog = db.tblAdmissionCommLogs.ToList().Where(x => x.IDAdmission == id).OrderByDescending(y => y.IDAdmissionCommLog).ToList();
                IEnumerable<tblPayment> payments = db.tblPayments.ToList().Where(x => x.IDAdmission == id).OrderByDescending(y => y.IDPayment).ToList();
                IEnumerable<tblAdmissionAttachment> attachments = db.tblAdmissionAttachments.ToList().Where(x => x.IDAdmission == id).OrderByDescending(y => y.IDAdmAttachment).ToList();
                tblRank rank = db.tblRanks.SingleOrDefault(x => x.IDRank == admission.IDRank);
                IEnumerable<tblRank> rankLists = db.tblRanks.ToList();
                IEnumerable<tblAdmissionAttachmentType> attachmentTypes = db.tblAdmissionAttachmentTypes.ToList();
                IEnumerable<tblPaymentMethod> paymentMethods = db.tblPaymentMethods.ToList();

                AdmissionNew adm = new AdmissionNew();
                adm.adm = admission;
                adm.rank = rank;
                adm.rankList = rankLists;

                AdmissionBillingNew admBilling = new AdmissionBillingNew();
                admBilling.admissionData = admission;
                admBilling.admBillingList = admissionBilling;

                AssertionNew assertionNew = new AssertionNew();
                assertionNew.admission = admission;
                assertionNew.assertionLists = assertion;

                PaymentHistoryNew paymentNew = new PaymentHistoryNew();
                paymentNew.admission = admission;
                paymentNew.paymentLists = payments;

                VitalSignsNew vsn = new VitalSignsNew();
                vsn.admission = admission;
                vsn.vitalSignsLists = vitalSigns;

                CommLogNew cln = new CommLogNew();
                cln.admission = admission;
                cln.commLogLists = commLog;

                AttachmentNew an = new AttachmentNew();
                an.admission = admission;
                an.attachmentLists = attachments;
                an.attachmentTypes = attachmentTypes;


                ViewBag.ranks = new SelectList(db.tblRanks, "IDRank", "Rank");
                ViewBag.assertions = new SelectList(db.tblAssertionCategories, "IDAssertionCategory", "Category");
                ViewBag.paymentMethods = new SelectList(db.tblPaymentMethods, "IDPaymentMethod", "PaymentMethod");

                AdmissionDetails ad = new AdmissionDetails();
                ad.Admissions = adm;
                ad.admissiongBillingNew = admBilling;
                ad.Assertion = assertionNew;
                ad.VitalSigns = vsn;
                ad.CommLog = cln;
                ad.Payments = paymentNew;
                ad.Attachments = an;
                ad.Rank = Rank;
                ad.rankLists = rankLists;

                //var admissionBilling = db.tblAdmissionBillings.Where(x => x.IDAdmission == id);
                //Response.Write(admissionBilling);
                if (admission == null)
                {
                    return HttpNotFound();
                }
                return View(ad);
            }
            return RedirectToAction("Login", "Login");

        }

        // GET: Admissions/Create
        public ActionResult Create()
        {
            if (Session["Username"] != null)
            {
                return View();
            }
            else {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: Admissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDAdmission,IDResident,Resident,Nickname,AdmissionDate,TerminationDate,TreatmentFee,TotalBilling,TotalPaid,OverallBalance,IsActive,LastPaymentInfo,StopTFBilling,rank,Phase")] tblAdmission tblAdmission)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.tblAdmissions.Add(tblAdmission);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else {
                return RedirectToAction("Login", "Login");
            }
            
            return View(tblAdmission);
        }

        // GET: Admissions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login","Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAdmission admission = db.tblAdmissions.SingleOrDefault(m => m.IDAdmission == id);
            if (admission == null)
            {
                return HttpNotFound();
            }
            return View(admission);
        }

        // POST: Admissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDAdmission,IDResident,Resident,Nickname,AdmissionDate,TerminationDate,TreatmentFee,TotalBilling,TotalPaid,OverallBalance,IsActive,LastPaymentInfo,StopTFBilling,rank,Phase")] tblAdmission admission)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(admission).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else {
                return RedirectToAction("Login", "Login");
            }
            
            return View(admission);
        }

        // GET: Admissions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Username"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblAdmission tbladmission = db.tblAdmissions.Find(id);
                if (tbladmission == null)
                {
                    return HttpNotFound();
                }
                return View(tbladmission);
            }
            else {
                return RedirectToAction("Login", "Login");
            }
            
        }

        // POST: Admissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null)
            {
                tblAdmission admission = db.tblAdmissions.Find(id);
                db.tblAdmissions.Remove(admission);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else {
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
