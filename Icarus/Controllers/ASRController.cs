using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rotativa;
using Icarus.Models;
using System.Web.Mvc;
namespace Icarus.Controllers
{
    public class ASRController : Controller
    {
        // GET: ASR
        public ActionResult Index()
        {
            using (ICARUSDBEntities db = new ICARUSDBEntities())
            {
                var employeeList = db.vrptAssertionSummaries.ToList();
                return View(employeeList);
            }
        }

        //Convert Index Page as PDF
        public ActionResult PrintViewToPdf()
        {
            var report = new ActionAsPdf("Index");
            return report;
        }
    }
}