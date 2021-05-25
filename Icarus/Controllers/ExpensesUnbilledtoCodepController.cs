using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rotativa;
using Icarus.Models;
namespace Icarus.Controllers
{
    public class ExpensesUnbilledtoCodepController : Controller
    {
        // GET: UnbilledtoCodep
        private ICARUSDBEntities db = new ICARUSDBEntities();

        [HttpGet]
        public ActionResult Index()
        {
            if (Session["Username"] != null)
            {
                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                var report = new ViewAsPdf(db.vrptExpenses.ToList().Where(x => x.ChargeToCodep == "N").OrderBy(x => x.ExpenseDate).ToList())
                {
                    PageOrientation = Rotativa.Options.Orientation.Landscape,
                    PageSize = Rotativa.Options.Size.A4,
                    CustomSwitches = footer
                };
                return report;
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