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

        public ActionResult AdmissionDetails(int? id)
        {
            if (Session["Username"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblAdmission tblAdmission = db.tblAdmissions.Find(id);
                if (tblAdmission == null)
                {
                    return HttpNotFound();
                }
                return View("AdmissionDetails", tblAdmission);
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

                IEnumerable<tblAdmissionBilling> admissionBilling = db.tblAdmissionBillings.ToList().Where(x => x.IDAdmission == id).OrderByDescending(y => y.IDAdmissionBilling).ToList();
                tblAdmissionBilling adBilling = new tblAdmissionBilling();
                adBilling.IDAdmission = admission.IDAdmission;

                IEnumerable<tblAssertionCategory> assertionListsCategory = db.tblAssertionCategories.ToList();
                IEnumerable<tblAssertion> assertionLists = db.tblAssertions.ToList().Where(x => x.IDAdmission == id).OrderByDescending(y => y.IDAssertion).ToList();
                tblAssertion assertion = new tblAssertion();
                assertion.IDAdmission = admission.IDAdmission;

                IEnumerable<tblPayment> payments = db.tblPayments.ToList().Where(x => x.IDAdmission == id).OrderByDescending(y => y.IDPayment).ToList();
                IEnumerable<tblPaymentMethod> paymentMethod = db.tblPaymentMethods.ToList();

                IEnumerable<tblAdmissionVitalSign> vitalSignsLists = db.tblAdmissionVitalSigns.ToList().Where(x => x.IDAdmission == id).OrderByDescending(y => y.IDVitalSign).ToList();
                tblAdmissionVitalSign vitalSigns = new tblAdmissionVitalSign();
                vitalSigns.IDAdmission = admission.IDAdmission;

                IEnumerable<tblAdmissionCommLog> commLogList = db.tblAdmissionCommLogs.ToList().Where(x => x.IDAdmission == id).OrderByDescending(y => y.IDAdmissionCommLog).ToList();
                tblAdmissionCommLog commLog = new tblAdmissionCommLog();
                commLog.IDAdmission = admission.IDAdmission;

                tblResident resident = db.tblResidents.SingleOrDefault(x => x.IDResident == admission.IDResident);

                tblAdmissionAttachment attachments = new tblAdmissionAttachment();
                attachments.IDAdmission = admission.IDAdmission;

                IEnumerable<tblAdmissionAttachment> attachmentLists = db.tblAdmissionAttachments.ToList().Where(x => x.IDAdmission == id).OrderByDescending(y => y.IDAdmAttachment).ToList();
                IEnumerable<tblAdmissionAttachmentType> attachmentTypes = db.tblAdmissionAttachmentTypes.ToList();

                ViewBag.ranks = new SelectList(db.tblRanks, "IDRank", "Rank");

                ViewBag.assertions = new SelectList(db.tblAssertionCategories, "IDAssertionCategory", "Category");

                ViewBag.paymentMethods = new SelectList(db.tblPaymentMethods, "IDPaymentMethod", "PaymentMethod");

                ViewBag.attachmentType = new SelectList(db.tblAdmissionAttachmentTypes, "IDAttachmentType", "AttachmentType");

                ViewBag.residentName = resident.Firstname + " " + resident.Nickname + " " + resident.Lastname;


                
                ViewData["AssertionCategories"] = assertionListsCategory;
                ViewData["PaymentMethod"] = paymentMethod;
                ViewData["AdmissionBilling"] = admissionBilling;
                ViewData["AssertionsLists"] = assertionLists;
                ViewData["vitalSigns"] = vitalSignsLists;
                ViewData["CommLogLists"] = commLogList;
                ViewData["AttachmentLists"] = attachmentLists;

                ViewBag.admissionBillingLists = true;
                ViewBag.assertionLists = true;
                ViewBag.vitalSignsLists = true;
                ViewBag.commLogCheck = true;
                ViewBag.attachmentList = true;


                if (!attachmentLists.Any() || attachmentLists == null)
                {
                    ViewBag.attachmentList = false;
                }

                if (!admissionBilling.Any() || admissionBilling == null)
                {
                    ViewBag.admissionBillingLists = false;
                }


                if (!assertionLists.Any() || assertionLists == null)
                {
                    ViewBag.assertionLists = false;
                }

                if (!vitalSignsLists.Any() || vitalSignsLists == null)
                {
                    ViewBag.vitalSignsLists = false;
                }

                if (!commLogList.Any() || commLogList == null)
                {
                    ViewBag.commLogCheck = false;
                }


                ViewBag.generatedBy = Session["Username"];

                AdmissionDetails ad = new AdmissionDetails();
                ad.Admissions = admission;
                ad.AdmissionBilling = adBilling;
                ad.Assertion = assertion;
                ad.VitalSigns = vitalSigns;
                ad.CommLog = commLog;
                ad.Payments = payments;
                ad.Attachments = attachments;

                if (admission == null)
                {
                    return HttpNotFound();
                }
                return View(ad);
            }
            return RedirectToAction("Login", "Login");

        }

        [HttpPost, ActionName("CreateAssertion")]
        [ValidateAntiForgeryToken]
        public ActionResult AssertionCreate([Bind(Include = "IDAssertion,Description,IDAdmission,AssertionDate,IDAssertionCategory,Qty,Price,Markup,MarkupValue,SubTotal,Notes,PostedDate")] tblAssertion tblAssertion) {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.tblAssertions.Add(tblAssertion);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Admissions", new { id = tblAssertion.IDAdmission });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

            return RedirectToAction("Details", "Admissions", new { id = tblAssertion.IDAdmission });
        }

        [HttpPost, ActionName("CreateTreatmentFee")]
        [ValidateAntiForgeryToken]
        public ActionResult TreatmentFeeCreate([Bind(Include = "IDAdmissionBilling,IDAdmission,BillingDate,GeneratedDate,Amount,Notes,PostedDate,GeneratedBy")] tblAdmissionBilling tblAdmissionBilling)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    tblAdmissionBilling.GeneratedBy = Session["Username"].ToString();
                    db.tblAdmissionBillings.Add(tblAdmissionBilling);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Admissions", new { id = tblAdmissionBilling.IDAdmission });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

            return RedirectToAction("Details", "Admissions", new { id = tblAdmissionBilling.IDAdmission });
        }

        [HttpPost, ActionName("CreateCommLog")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCommLog([Bind(Include = "IDAdmissionCommLog,PostedBy,IDAdmission,LogDate,Details")] tblAdmissionCommLog tblAdmissionCommLog)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    tblAdmissionCommLog.PostedBy = Session["Username"].ToString();
                    db.tblAdmissionCommLogs.Add(tblAdmissionCommLog);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Admissions", new { id = tblAdmissionCommLog.IDAdmission });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            return RedirectToAction("Details", "Admissions", new { id = tblAdmissionCommLog.IDAdmission });

        }

        [HttpPost, ActionName("CreateVitalSigns")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVitalSigns([Bind(Include = "IDVitalSign,IDAdmission,Performed,BloodPressure,Temperature,PulseRate,Weight")] tblAdmissionVitalSign tblAdmissionVitalSign)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.tblAdmissionVitalSigns.Add(tblAdmissionVitalSign);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Admissions", new { id = tblAdmissionVitalSign.IDAdmission });

                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            return RedirectToAction("Details", "Admissions", new { id = tblAdmissionVitalSign.IDAdmission });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePayments([Bind(Include = "IDPayment,PaidDate,IDAdmission,TotalPaid,IDPaymentMethod,Bank,CheckNo,CheckDate,Notes,isVerified,PostedDate")] tblPayment tblPayment)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.tblPayments.Add(tblPayment);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Admissions", new { id = tblPayment.IDAdmission });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            return RedirectToAction("Details", "Admissions", new { id = tblPayment.IDAdmission });

        }

        // GET: Admissions/Create
        public ActionResult Create()
        {
            if (Session["Username"] != null)
            {
                var residents = db.tblResidents.Select(
                    s => new {
                        Text = s.Firstname + " '" + s.Nickname + "' " + s.Lastname,
                        Value = s.IDResident
                    }
                ).ToList();
                ViewBag.residentList = new SelectList(residents, "Value", "Text");
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAttachment([Bind(Include = "IDAdmAttachment,IDAdmission,Description,Filename,UploadedFile,IDAttachmentType")] tblAdmissionAttachment tblAdmissionAttachment, HttpPostedFileBase file)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.tblAdmissionAttachments.Add(tblAdmissionAttachment);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Admissions", new { id = tblAdmissionAttachment.IDAdmission });

                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

            return RedirectToAction("Details", "Admissions", new { id = tblAdmissionAttachment.IDAdmission });
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
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDAdmission,IDResident,AdmissionDate,TerminationDate,IsActive,Notes,StopTFBilling,TreatmentFee,TotalPaid,TotalBilling,OverallBalance,Phase,IDRank,Status")] tblAdmission tbladmission)
        {

            if (ModelState.IsValid)
            {
                db.Entry(tbladmission).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index","Admissions");
            }
            return View(tbladmission);

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

        public virtual PartialViewResult Assertions()
        {
            
            return PartialView("~/Views/Admissions/_Assertions.cshtml");
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
