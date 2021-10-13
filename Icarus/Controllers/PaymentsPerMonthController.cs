using System;
using System.Linq;
using System.Web.Mvc;
using Rotativa;
using Icarus.Models;
namespace Icarus.Controllers

{
    public class PaymentsPerMonthController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        [Route("PaymentsPerMonth")]
        [HttpGet]
        // GET: PaymentsPerMonth
        public ActionResult Index(DateTime datefrom, DateTime? dateto)
        {
            try
            {
                if (datefrom != null && dateto == null)
                {
                    string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                    var report = new ViewAsPdf(db.tblPayments.ToList().Where(x => x.PaidDate.Year == datefrom.Year && x.IsVerified == "Y").OrderBy(x => x.PaidDate).ToList())
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
                    var report = new ViewAsPdf(db.tblPayments.ToList().Where(x => x.PaidDate >= firstDay && x.PaidDate <= secondDay && x.IsVerified == "Y").OrderBy(x => x.PaidDate).ToList())
                    {
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        PageSize = Rotativa.Options.Size.A4,
                        CustomSwitches = footer
                    };
                    return report;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception " + e);
            }
            return HttpNotFound();
        }
    }
}