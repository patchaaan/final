using System;
using System.Data.Entity;
using System.Linq;
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
                if (Session["isADG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y")
                {
                    try {
                        int chart = db.tblExpensesChartOfAccounts.Max(x => x.IDAccount);
                        tblExpensesChartOfAccount chartOfAccount = new tblExpensesChartOfAccount();
                        chartOfAccount.IDAccount = chart + 1;
                        ViewData["Chart"] = chartOfAccount;
                        return View(db.tblExpensesChartOfAccounts.ToList());
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

        // POST: ExpensesChartOfAccounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDAccount,AccountCode,Account")] tblExpensesChartOfAccount tblExpensesChartOfAccount)
        {
            if (Session["Username"] != null) {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.tblExpensesChartOfAccounts.Add(tblExpensesChartOfAccount);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception " + e);
                    }
                }

                return View(tblExpensesChartOfAccount);
            }
            return RedirectToAction("Login", "Login");

        }

        [HttpGet]
        public PartialViewResult EditPartial(int id)
        {
            tblExpensesChartOfAccount chartofaccount = new tblExpensesChartOfAccount();
            try
            {
                chartofaccount = db.tblExpensesChartOfAccounts.Find(id);
                return PartialView("_EditPartial", chartofaccount);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception " + e);
            }
            return PartialView("_EditPartial", chartofaccount);
        }

        // POST: ExpensesChartOfAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDAccount,AccountCode,Account")] tblExpensesChartOfAccount tblExpensesChartOfAccount)
        {
            if (Session["Username"] != null) {
                if (Session["isADG"].ToString() == "Y" || Session["isAAG"].ToString() == "Y")
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            db.Entry(tblExpensesChartOfAccount).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("Exception " + e);
                        }
                    }
                    return View(tblExpensesChartOfAccount);
                }
                return RedirectToAction("Index", "Residents");
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: ExpensesChartOfAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Username"] != null) {
                try
                {
                    tblExpensesChartOfAccount tblExpensesChartOfAccount = db.tblExpensesChartOfAccounts.Find(id);
                    db.tblExpensesChartOfAccounts.Remove(tblExpensesChartOfAccount);
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
