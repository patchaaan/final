using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icarus.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        [Route("Reports/")]
        public ActionResult Index()
        {
            return View();
        }
    }
}