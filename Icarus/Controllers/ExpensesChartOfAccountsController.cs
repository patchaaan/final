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
    public class ExpensesChartOfAccountsController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: ExpensesChartOfAccounts
        [Route("ChartOfAccounts/")]
        public ActionResult Index()
        {
            if (Session["Username"] != null) {
                return View(db.tblExpensesChartOfAccounts.ToList());
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: ExpensesChartOfAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Username"] != null) {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblExpensesChartOfAccount tblExpensesChartOfAccount = db.tblExpensesChartOfAccounts.Find(id);
                if (tblExpensesChartOfAccount == null)
                {
                    return HttpNotFound();
                }
                return View(tblExpensesChartOfAccount);
            }
            return RedirectToAction("Login", "Login");

        }

        // GET: ExpensesChartOfAccounts/Create
        public ActionResult Create()
        {
            if (Session["Username"] != null) {
                return View();
            }
            return RedirectToAction("Login", "Login");

        }

        // POST: ExpensesChartOfAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDAccount,AccountCode,Account")] tblExpensesChartOfAccount tblExpensesChartOfAccount)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    db.tblExpensesChartOfAccounts.Add(tblExpensesChartOfAccount);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(tblExpensesChartOfAccount);
            }
            return RedirectToAction("Login", "Login");

        }

        // GET: ExpensesChartOfAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Username"] != null) {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblExpensesChartOfAccount tblExpensesChartOfAccount = db.tblExpensesChartOfAccounts.Find(id);
                if (tblExpensesChartOfAccount == null)
                {
                    return HttpNotFound();
                }
                return View(tblExpensesChartOfAccount);
            }
            return RedirectToAction("Login", "Login");

        }

        // POST: ExpensesChartOfAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDAccount,AccountCode,Account")] tblExpensesChartOfAccount tblExpensesChartOfAccount)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    db.Entry(tblExpensesChartOfAccount).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(tblExpensesChartOfAccount);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: ExpensesChartOfAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Username"] != null) {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblExpensesChartOfAccount tblExpensesChartOfAccount = db.tblExpensesChartOfAccounts.Find(id);
                if (tblExpensesChartOfAccount == null)
                {
                    return HttpNotFound();
                }
                return View(tblExpensesChartOfAccount);
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: ExpensesChartOfAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null) {
                tblExpensesChartOfAccount tblExpensesChartOfAccount = db.tblExpensesChartOfAccounts.Find(id);
                db.tblExpensesChartOfAccounts.Remove(tblExpensesChartOfAccount);
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
