using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rotativa;
using Icarus.Models;
namespace Icarus.Controllers
{
    public class UnbilledtoCodepController : Controller
    {
        // GET: UnbilledtoCodep
        public ActionResult Index()
        {
            using (ICARUSDBEntities db = new ICARUSDBEntities())
            {
                var employeeList = db.vrptUnderChargedExpenses.ToList();
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