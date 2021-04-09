using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Icarus.Models;
using Rotativa;
namespace Icarus.Controllers
{
    public class CollectiblesController : Controller
    {
        // GET: Collectibles
        public ActionResult Index()
        {
            using (ICARUSDBEntities db = new ICARUSDBEntities())
            {
                var employeeList = db.vrptCollectibles.ToList();
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