using System;
using System.Linq;
using System.Web.Mvc;
using Rotativa;
using Icarus.Models;
namespace Icarus.Controllers
{
    public class ExpensesUnbilledtoCodepController : Controller
    {
        // GET: UnbilledtoCodep
        private ICARUSDBEntities db = new ICARUSDBEntities();

        [Route("ExpensesUnbilledtoCodep")]
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["Username"] != null)
            {
                try
                {
                    string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                    var report = new ViewAsPdf(db.vrptUnderChargedExpenses.ToList().OrderBy(x => x.ExpenseDate).ToList())
                    {
                        PageOrientation = Rotativa.Options.Orientation.Landscape,
                        PageSize = Rotativa.Options.Size.A4,
                        CustomSwitches = footer
                    };
                    return report;
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