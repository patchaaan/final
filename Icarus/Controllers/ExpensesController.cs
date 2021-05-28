﻿using System;
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
    public class ExpensesController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: Expenses
        [Route("Expenses/")]
        [HttpGet]
        public ActionResult Index(DateTime? start, DateTime? end)
        {
            if (Session["Username"] != null)
            {
                if (Session["isADG"].ToString() == "Y" || Session["isEDG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y")
                {
                    var total = db.tblExpenses.ToList().Select(x => x.Amount).ToList().Sum();
                    ViewBag.totalAmount = Math.Round((Double)total, 2);

                    if (start != null && end != null)
                    {
                        return View(db.vExpensesBrowses.Where(x => x.ExpenseDate >= start && x.ExpenseDate <= end).ToList().OrderByDescending(y => y.ExpenseDate).ToList());
                    } else if (start != null && end == null) {
                        return View(db.vExpensesBrowses.Where(x => x.ExpenseDate >= start).ToList().OrderBy(y => y.ExpenseDate).ToList());
                    } else
                    {
                        var firstDay = new DateTime(DateTime.Now.Year, 1, 1);
                        var secondDay = new DateTime(DateTime.Now.Year, 12, 31);
                        return View(db.vExpensesBrowses.Where(y => y.ExpenseDate >= firstDay && y.ExpenseDate <= secondDay).ToList().OrderByDescending(y => y.IDExpense).ToList());
                    }
                }
                return RedirectToAction("Index","Residents");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // GET: Expenses/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Username"] != null) {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblExpens tblExpens = db.tblExpenses.Find(id);
                if (tblExpens == null)
                {
                    return HttpNotFound();
                }
                return View(tblExpens);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: Expenses/Create
        public ActionResult Create()
        {
            if (Session["Username"] != null) {
                if (Session["isAAG"].ToString() == "Y" || Session["isADG"].ToString() == "Y") {
                    var account = db.tblExpensesChartOfAccounts.Select(
                            s => new {
                                Text = s.Account + "      " + s.AccountCode,
                                Value = s.IDAccount
                            }
                        ).ToList();
                    var residents = db.vAdmissionBrowses.Select(
                            s => new {
                                Text = s.Resident,
                                Value = s.IDAdmission
                            }
                        ).ToList();
                    int idCTC = db.tblExpensesForAssertions.Max(x => x.IDChargeToCodep);
                    int idExp = db.tblExpenses.Max(x => x.IDExpense);
                    tblExpens expenses = new tblExpens();
                    expenses.IDExpense = idExp + 1;
                    ViewBag.residentList = new SelectList(residents, "Value", "Text");
                    ViewBag.accountsList = new SelectList(account, "Value", "Text");
                    ViewBag.vendors = new SelectList(db.tblVendors, "IDVendor", "Vendor");
                    ViewBag.category = new SelectList(db.tblAssertionCategories, "IDAssertionCategory", "Category");
                    ViewBag.lastIDCTC = idCTC + 1;
                    ViewBag.lastIDEXP = idExp + 1;
                    return View(expenses);
                }
                return RedirectToAction("Index","Residents");
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: Expenses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDExpense,DatePosted,ExpenseDate,ORNumber,IDVendor,Particulars,WithReceipt,IDAccount,EncodedBy,IsVerified,ChargeToCodep,VATSales,VATAmount,VATExempt,Amount,PostedDate,ChargedToCodep,TIN")] tblExpens tblExpens)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    tblExpens.EncodedBy = Session["Username"].ToString();
                    tblExpens.DatePosted = tblExpens.PostedDate;
                    db.tblExpenses.Add(tblExpens);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(tblExpens);
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        public JsonResult CreateAssertion(tblAssertion tblAssertion) {
            if (ModelState.IsValid)
            {
                using (ICARUSDBEntities icarus = new ICARUSDBEntities())
                {
                    db.tblAssertions.Add(tblAssertion);
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("[dbo].[spRecalcAdmissionBalance] @IDAdmission", new SqlParameter("IDAdmission", tblAssertion.IDAdmission));
                    int id = tblAssertion.IDAssertion;
                    return Json(id, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Failed", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateExpensesAssertion(tblExpensesForAssertion tblExpensesForAssertion)
        {
            if (ModelState.IsValid)
            {
                db.tblExpensesForAssertions.Add(tblExpensesForAssertion);
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            return Json("Failed", JsonRequestBehavior.AllowGet);
        }

        // GET: Expenses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Username"] != null) {
                if (Session["isADG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y") {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    tblExpens tblExpens = db.tblExpenses.Find(id);
                    int idexpense = tblExpens.IDExpense;
                    if (tblExpens == null)
                    {
                        return HttpNotFound();
                    }
                    tblExpensesForAssertion expenseasertion = db.tblExpensesForAssertions.Where(x => x.IDExpense == tblExpens.IDExpense).FirstOrDefault();
                    if (expenseasertion == null)
                    {
                        tblExpensesForAssertion newexpense = new tblExpensesForAssertion();
                        int idCTC = db.tblExpensesForAssertions.Max(x => x.IDChargeToCodep);
                        newexpense.IDChargeToCodep = idCTC + 1;
                        newexpense.IDExpense = idexpense;
                        ViewBag.idctc = idCTC + 1;
                        ViewBag.idexp = tblExpens.IDExpense;
                        ViewBag.EmptyAssertion = true;
                        ViewData["Assertions"] = null;
                    }
                    else {
                        IEnumerable<tblAssertion> assertions = db.tblAssertions.ToList().Where(x => x.IDChargeToCodep == expenseasertion.IDChargeToCodep).ToList();
                        ViewBag.EmptyAssertion = false;
                        ViewBag.idctc = expenseasertion.IDChargeToCodep;
                        ViewBag.idexp = expenseasertion.IDExpense;
                        ViewBag.assertioncategory = expenseasertion.IDAssertionCategory;
                        ViewBag.description = expenseasertion.Description;
                        ViewBag.chargeDate = expenseasertion.ChargeDate;
                        ViewData["Assertions"] = assertions;
                    }

                    ViewData["ExpenseAssertion"] = expenseasertion;
                    var account = db.tblExpensesChartOfAccounts.Select(
                            s => new {
                                Text = s.Account + "      " + s.AccountCode,
                                Value = s.IDAccount
                            }
                        ).ToList();
                    var residents = db.vAdmissionBrowses.Select(
                            s => new {
                                Text = s.Resident,
                                Value = s.IDAdmission
                            }
                        ).ToList();
                    ViewBag.residentList = new SelectList(residents, "Value", "Text");
                    ViewBag.accountsList = new SelectList(account, "Value", "Text");
                    ViewBag.category = new SelectList(db.tblAssertionCategories, "IDAssertionCategory", "Category");
                    ViewBag.vendors = new SelectList(db.tblVendors, "IDVendor", "Vendor");
                    return View(tblExpens);
                }
                return RedirectToAction("Index", "Residents");
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpPost, ActionName("EditAssertion")]
        public JsonResult EditAssertion([Bind(Include = "IDAssertion,Description,IDAdmission,AssertionDate,IDAssertionCategory,IDChargeToCodep,Qty,Price,Markup,MarkupValue,SubTotal,Notes,PostedDate")] tblAssertion tblAssertion)
        {
            if (ModelState.IsValid)
            {
                    db.Entry(tblAssertion).State = EntityState.Modified;
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("[dbo].[spRecalcAdmissionBalance] @IDAdmission", new SqlParameter("IDAdmission", tblAssertion.IDAdmission));
                return Json("Sucess");
            }
            return Json("Failed");
        }

        [HttpGet]
        public PartialViewResult DetailsPartial(int id)
        {
            tblExpens expense = db.tblExpenses.Find(id);
            var account = db.tblExpensesChartOfAccounts.Select(
                        s => new {
                            Text = s.Account + "      " + s.AccountCode,
                            Value = s.IDAccount
                        }
                    ).ToList();
            var residents = db.vAdmissionBrowses.Select(
                    s => new {
                        Text = s.Resident,
                        Value = s.IDAdmission
                    }
                ).ToList();
            ViewBag.residentList = new SelectList(residents, "Value", "Text");
            ViewBag.accountsList = new SelectList(account, "Value", "Text");
            ViewBag.vendors = new SelectList(db.tblVendors, "IDVendor", "Vendor");
            ViewBag.category = new SelectList(db.tblAssertionCategories, "IDAssertionCategory", "Category");
            return PartialView("_DetailsPartial", expense);
        }


        [HttpGet]
        public PartialViewResult EditAssertionPartial(int id)
        {
            var account = db.tblExpensesChartOfAccounts.Select(
                        s => new {
                            Text = s.Account + "      " + s.AccountCode,
                            Value = s.IDAccount
                        }
                    ).ToList();
            var residents = db.vAdmissionBrowses.Select(
                    s => new {
                        Text = s.Resident,
                        Value = s.IDAdmission
                    }
                ).ToList();
            ViewBag.residentList = new SelectList(residents, "Value", "Text");
            ViewBag.accountsList = new SelectList(account, "Value", "Text");
            ViewBag.vendors = new SelectList(db.tblVendors, "IDVendor", "Vendor");
            ViewBag.category = new SelectList(db.tblAssertionCategories, "IDAssertionCategory", "Category");
            tblAssertion tblAssertion = db.tblAssertions.Find(id);
            return PartialView("_EditAssertionPartial", tblAssertion);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDExpense,DatePosted,ExpenseDate,ORNumber,IDVendor,Particulars,WithReceipt,IDAccount,EncodedBy,IsVerified,ChargeToCodep,VATSales,VATAmount,VATExempt,Amount,PostedDate,ChargedToCodep,TIN")] tblExpens tblExpens)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    tblExpens.EncodedBy = Session["Username"].ToString();
                    db.Entry(tblExpens).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(tblExpens);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: Expenses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Username"] != null) {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblExpens tblExpens = db.tblExpenses.Find(id);
                if (tblExpens == null)
                {
                    return HttpNotFound();
                }
                return View(tblExpens);
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null) {
                tblExpens tblExpens = db.tblExpenses.Find(id);
                db.tblExpenses.Remove(tblExpens);
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
