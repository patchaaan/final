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

        // GET: tblExpensesChartOfAccounts
        public ActionResult Index()
        {
            return View(db.tblExpensesChartOfAccounts.ToList());
        }

        // GET: tblExpensesChartOfAccounts/Details/5
        public ActionResult Details(int? id)
        {
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

        // GET: tblExpensesChartOfAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tblExpensesChartOfAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDAccount,AccountCode,Account")] tblExpensesChartOfAccount tblExpensesChartOfAccount)
        {
            if (ModelState.IsValid)
            {
                db.tblExpensesChartOfAccounts.Add(tblExpensesChartOfAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblExpensesChartOfAccount);
        }

        // GET: tblExpensesChartOfAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
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

        // POST: tblExpensesChartOfAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDAccount,AccountCode,Account")] tblExpensesChartOfAccount tblExpensesChartOfAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblExpensesChartOfAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblExpensesChartOfAccount);
        }

        // GET: tblExpensesChartOfAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
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

        // POST: tblExpensesChartOfAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblExpensesChartOfAccount tblExpensesChartOfAccount = db.tblExpensesChartOfAccounts.Find(id);
            db.tblExpensesChartOfAccounts.Remove(tblExpensesChartOfAccount);
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
