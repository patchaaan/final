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
                if (start != null && end != null)
                {
                    Response.Write("<script>console.log('Reading')</script>");
                    List<tblExpens> tblexp = new List<tblExpens>();
                    tblexp = db.tblExpenses.ToList().OrderByDescending(p => p.IDExpense).ToList();
                    return View(db.tblExpenses.Where(x => x.ExpenseDate >= start && x.ExpenseDate <= end).ToList().OrderBy(y => y.ExpenseDate).ToList());
                }
                else {
                    var firstDay = new DateTime(DateTime.Now.Year, 1, 1);
                    var secondDay = new DateTime(DateTime.Now.Year, 1, 19);
                    return View(db.tblExpenses.Where(y => y.ExpenseDate >= firstDay && y.ExpenseDate <= secondDay).ToList());
                }
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
                ViewBag.vendors = new SelectList(db.tblVendors, "IDVendor", "Vendor");
                return View();
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
                    db.tblExpenses.Add(tblExpens);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(tblExpens);
            }
            return RedirectToAction("Login", "Login");
        }

        // GET: Expenses/Edit/5
        public ActionResult Edit(int? id)
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
                ViewBag.vendors = new SelectList(db.tblVendors, "IDVendor", "Vendor");
                return View(tblExpens);
            }
            return RedirectToAction("Login", "Login");
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
