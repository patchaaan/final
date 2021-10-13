using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Icarus.Models;
using Rotativa;


namespace Icarus.Controllers
{
    public class ReportsController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: Report
        [Route("Reports/")]
        public ActionResult Index()
        {
            if (Session["Username"] != null) {
                return View();
            }

            return RedirectToAction("Login","Login");
        }

        public ActionResult AssertionSummary(DateTime? datefrom, DateTime? dateto)
        {
            if (Session["Username"] != null)
            {
                try
                {
                    if (datefrom != null && dateto == null)
                    {
                        return View("_AssertionSummary", db.vrptAssertionSummaries.ToList().Where(x => x.AssertionDate >= datefrom).ToList());
                    }
                    else
                    {
                        return View("_AssertionSummary", db.vrptAssertionSummaries.ToList().Where(x => x.AssertionDate >= datefrom && x.AssertionDate <= dateto).ToList());
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }
            }
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public ActionResult FileToPdf(DateTime? datefrom, DateTime? dateto)
        {
            if (Session["Username"] != null)
            {
                try
                {
                    if (datefrom != null && dateto == null)
                    {
                        var report = new ActionAsPdf("AssertionSummary", new { datefrom, dateto })
                        {
                            PageOrientation = Rotativa.Options.Orientation.Landscape,
                            PageSize = Rotativa.Options.Size.A4
                        };
                        return report;

                    }
                    else
                    {
                        var report = new ActionAsPdf("AssertionSummary", new { datefrom, dateto })
                        {
                            PageOrientation = Rotativa.Options.Orientation.Landscape,
                            PageSize = Rotativa.Options.Size.A4
                        };
                        return report;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Exception " + e);
                }
            }
            return RedirectToAction("Login", "Login");
        }
    }
}