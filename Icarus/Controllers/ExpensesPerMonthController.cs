using Icarus.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icarus.Controllers
{
    public class ExpensesPerMonthController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        [Route("ExpensesPerMonth")]
        // GET: ExpensesPerMonth
        [HttpGet]
        public ActionResult Index(DateTime datefrom, DateTime? dateto)
        {
            if (datefrom != null && dateto == null)
            {
                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                var report = new ViewAsPdf(db.vExpensesBrowses.ToList().Where(x => x.ExpenseDate == null ? false : ((DateTime)x.ExpenseDate).Year == datefrom.Year && x.IsVerified == "Y").OrderBy(x => x.ExpenseDate).ToList())
                {
                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    PageSize = Rotativa.Options.Size.A4,
                    CustomSwitches = footer
                };
                return report;
            }
            else
            {
                var firstDay = new DateTime(datefrom.Year, 1, 1);
                var secondDay = new DateTime(dateto.Value.Year, 12, 31);
                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                var report = new ViewAsPdf(db.vExpensesBrowses.ToList().Where(x => x.ExpenseDate >= firstDay && x.ExpenseDate <= secondDay && x.IsVerified == "Y").OrderBy(x => x.ExpenseDate).ToList())
                {
                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    PageSize = Rotativa.Options.Size.A4,
                    CustomSwitches = footer
                };
                return report;
            }
        }
    }
}