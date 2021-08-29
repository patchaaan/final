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
using Rotativa;
using System.IO;
using System.Security.Cryptography;
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
                var residents = db.tblResidents.OrderByDescending(x => x.IDResident).Select(
                    s => new
                    {
                        Text = s.Firstname + " '" + s.Nickname + "' " + s.Lastname,
                        Value = s.IDResident
                    }).ToList();
                ViewBag.residentList = new SelectList(residents, "Value", "Text");
                ViewBag.ranks = new SelectList(db.tblRanks, "IDRank", "Rank");
                var admis = db.tblAdmissions.ToList().OrderByDescending(x => x.IDAdmission).FirstOrDefault();

                if (admis == null) {
                    tblAdmission admission = new tblAdmission();
                    admission.IDAdmission = 1;
                    ViewData["Admissions"] = admission;
                }
                else {
                    tblAdmission admission = new tblAdmission();
                    admission.IDAdmission = admis.IDAdmission + 1;
                    ViewData["Admissions"] = admission;
                }
                
                var totalbilling = db.vAdmissionBrowses.ToList().Select(x => x.TotalBilling).ToList().Sum();
                ViewBag.totalActive = db.vAdmissionBrowses.Where(x => x.IsActive == "Y").ToList().Count();
                ViewBag.totalbilling = Math.Round((Double)totalbilling, 2);
                return View(db.vAdmissionBrowses.ToList().OrderByDescending(p => p.IDAdmission).ToList());
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpGet]
        public ActionResult GenerateBilling(int? id) {
            if (Session["Username"] != null) {
                DateTime today = DateTime.Today.AddDays(-30);
                tblAdmissionBilling billing = db.tblAdmissionBillings.Where(x => x.IDAdmission == id).OrderByDescending(x => x.BillingDate).FirstOrDefault();
                if (billing == null)
                {
                    ViewBag.Billing = null;
                    IEnumerable<tblAssertion> assertions = db.tblAssertions.ToList().Where(x => x.AssertionDate <= DateTime.Today && x.IDAdmission == id).ToList();
                    ViewData["Assertions"] = assertions;
                    ViewBag.TotalAssertion = assertions.Sum(x => x.SubTotal);
                    ViewBag.SumAll = assertions.Sum(x => x.SubTotal);
                }
                else {
                    ViewBag.Billing = billing;
                    //DateTime billingdate = new DateTime(billing.BillingDate.Year, billing.BillingDate.Month, billing.BillingDate.Day);
                    IEnumerable<tblAssertion> assertions = db.tblAssertions.ToList().Where(x => x.AssertionDate >= billing.BillingDate.Date.AddDays(-30) && x.AssertionDate <= DateTime.Today && x.IDAdmission == id).ToList();
                    ViewData["Assertions"] = assertions;
                    ViewBag.TotalAssertion = assertions.Sum(x => x.SubTotal);
                    ViewBag.SumAll = assertions.Sum(x => x.SubTotal) + billing.Amount;

                }
                tblPayment payment = db.tblPayments.Where(x => x.IDAdmission == id).OrderByDescending(x => x.PaidDate).FirstOrDefault();
                //vAdmissionBrowse browse = db.vAdmissionBrowses.Where(x => x.IDAdmission == id).FirstOrDefault();
                tblStaff staff = db.tblStaffs.Find(Int32.Parse(Session["ID"].ToString()));
                ViewBag.PreparedBy = staff.Firstname.ToString() + " " + staff.Lastname.ToString();
                if (payment != null)
                {
                    ViewBag.PaidDate = payment.PaidDate.ToString("MM/dd/yyyy");
                    ViewBag.TotalPaid = Math.Round((Double)payment.TotalPaid, 2);
                }
                else {
                    ViewBag.PaidDate = null;
                    ViewBag.TotalPaid = null;
                }

                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                var report = new ViewAsPdf(db.vAdmissionBrowses.Where(x => x.IDAdmission == id).FirstOrDefault())
                {
                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    PageSize = Rotativa.Options.Size.A4,
                    CustomSwitches = footer
                };
                return report;
            }
            else {
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

                tblAdmission admission = db.tblAdmissions.Find(id);
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

                ViewData["attachmentTypes"] = db.tblAdmissionAttachmentTypes.ToList();

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
                db.Database.ExecuteSqlCommand("[dbo].[spRecalcAdmissionBalance] @IDAdmission", new SqlParameter("IDAdmission", admission.IDAdmission));
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
                    db.Database.ExecuteSqlCommand("[dbo].[spRecalcAdmissionBalance] @IDAdmission", new SqlParameter("IDAdmission", tblAssertion.IDAdmission));
                    return RedirectToAction("Details", "Admissions", new { id = tblAssertion.IDAdmission });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

            return RedirectToAction("Details", "Admissions", new { id = tblAssertion.IDAdmission });
        }

        [HttpPost, ActionName("DeleteAssertion")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAssertion(int id)
        {
            if (Session["Username"] != null)
            {
                tblAssertion assertions = db.tblAssertions.Find(id);
                db.tblAssertions.Remove(assertions);
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("[dbo].[spRecalcAdmissionBalance] @IDAdmission", new SqlParameter("IDAdmission", assertions.IDAdmission));
                return RedirectToAction("Details", new { id = assertions.IDAdmission });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }

        [HttpPost, ActionName("DeleteVitalSign")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteVitalSign(int id)
        {
            if (Session["Username"] != null)
            {
                tblAdmissionVitalSign vitalSign = db.tblAdmissionVitalSigns.Find(id);
                db.tblAdmissionVitalSigns.Remove(vitalSign);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = vitalSign.IDAdmission });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }

        [HttpPost, ActionName("CommLogDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult CommLogDelete(int id)
        {
            if (Session["Username"] != null)
            {
                tblAdmissionCommLog commlog = db.tblAdmissionCommLogs.Find(id);
                db.tblAdmissionCommLogs.Remove(commlog);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = commlog.IDAdmission });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

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
                    db.Database.ExecuteSqlCommand("[dbo].[spRecalcAdmissionBalance] @IDAdmission", new SqlParameter("IDAdmission", tblAdmissionBilling.IDAdmission));
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
                if (Session["isADG"].ToString() == "Y" || Session["isEDG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y")
                {
                    var residents = db.tblResidents.Select(
                    s => new
                    {
                        Text = s.Firstname + " '" + s.Nickname + "' " + s.Lastname,
                        Value = s.IDResident
                    }).ToList();
                    ViewBag.residentList = new SelectList(residents, "Value", "Text");
                    ViewBag.ranks = new SelectList(db.tblRanks, "IDRank", "Rank");
                    return View();
                }
                return RedirectToAction("Index","Admissions");
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
        public ActionResult Create([Bind(Include = "IDAdmission,IDResident,AdmissionDate,TerminationDate,TreatmentFee,isActive,Notes,TotalBilling,TotalPaid,OverallBalance,StopTFBilling,Status,IDRank,Phase")] tblAdmission tblAdmission)
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
                return RedirectToAction("Details","Admissions", new { id = tbladmission.IDAdmission});
            }
            return View(tbladmission);
        }

        [HttpGet]
        public PartialViewResult EditCommLogPartial(int id)
        {
            tblAdmissionCommLog commlog = db.tblAdmissionCommLogs.Find(id);
            return PartialView("_EditcommLogPartial", commlog);
        }

        [HttpPost, ActionName("EditCommLog")]
        [ValidateAntiForgeryToken]
        public ActionResult CommLogEdit([Bind(Include = "IDAdmissionCommLog,PostedBy,IDAdmission,LogDate,Details")] tblAdmissionCommLog tblAdmissionCommLog)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(tblAdmissionCommLog).State = EntityState.Modified;
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


        [HttpGet]
        public PartialViewResult EditAssertionPartial(int id)
        {
            tblAssertion assertion = db.tblAssertions.Find(id);
            ViewBag.assertions = new SelectList(db.tblAssertionCategories, "IDAssertionCategory", "Category");
            return PartialView("_EditAssertionPartial", assertion);
        }

        [HttpPost, ActionName("EditAssertion")]
        [ValidateAntiForgeryToken]
        public ActionResult EditAssertion([Bind(Include = "IDAssertion,Description,IDAdmission,AssertionDate,IDAssertionCategory,Qty,Price,Markup,MarkupValue,SubTotal,Notes,PostedDate")] tblAssertion tblAssertion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblAssertion).State = EntityState.Modified;
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("[dbo].[spRecalcAdmissionBalance] @IDAdmission", new SqlParameter("IDAdmission", tblAssertion.IDAdmission));
                return RedirectToAction("Details", "Admissions", new { id = tblAssertion.IDAdmission });
            }
            return View(tblAssertion);
        }

        [HttpGet]
        public PartialViewResult EditVitalSignPartial(int id)
        {
            tblAdmissionVitalSign vitalsign = db.tblAdmissionVitalSigns.Find(id);
            return PartialView("_EditVitalSignPartial", vitalsign);
        }

        [HttpPost, ActionName("EditVitalSigns")]
        [ValidateAntiForgeryToken]
        public ActionResult EditVitalSigns([Bind(Include = "IDVitalSign,IDAdmission,Performed,BloodPressure,Temperature,PulseRate,Weight")] tblAdmissionVitalSign tblAdmissionVitalSign)
        {
            if (Session["Username"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(tblAdmissionVitalSign).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Admissions", new { id = tblAdmissionVitalSign.IDAdmission });

                }
            }
            return View(tblAdmissionVitalSign);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FileAttachment(HttpPostedFileBase file, FormCollection data, tblAdmissionAttachment dbfiles)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var filename = Path.GetFileName(file.FileName);
                    var fileExtension = Path.GetExtension(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/uploads"), filename);
                    var enc = Path.Combine(Server.MapPath("~/Content/uploads"), fileExtension);
                    file.SaveAs(path);

                    dbfiles.Description = data["Description"];
                    dbfiles.Filename = filename;

                    db.tblAdmissionAttachments.Add(dbfiles);
                    db.SaveChanges();
                    ViewBag.success = "File Uploaded!";
                    return RedirectToAction("Details", "Admissions", new { id = dbfiles.IDAdmission });
                }
                else
                {
                    ViewBag.error = "Select a File!";
                }
            }
            catch (Exception e)
            {
                ViewBag.error = "Error " + e;
            }
            return RedirectToAction("Details", "Admissions", new { id = dbfiles.IDAdmission });
        }

        [HttpPost, ActionName("DeleteAttachment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAttachment(int id) {
            if (Session["Username"] != null)
            {
                tblAdmissionAttachment attachment = db.tblAdmissionAttachments.Find(id);
                string file_name = attachment.Filename.ToString();
                string path = Server.MapPath("~/Content/uploads" + file_name);
                FileInfo file = new FileInfo(path);
                if (file.Exists)//check file exsit or not  
                {
                    file.Delete();
                }
                db.tblAdmissionAttachments.Remove(attachment);
                db.SaveChanges();
                return RedirectToAction("Details", "Admissions", new { id = attachment.IDAdmission });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }



        // GET: Admissions/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (Session["Username"] != null)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        tblAdmission tbladmission = db.tblAdmissions.Find(id);
        //        if (tbladmission == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(tbladmission);
        //    }
        //    else {
        //        return RedirectToAction("Login", "Login");
        //    }

        //}

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
