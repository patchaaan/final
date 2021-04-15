using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icarus.Controllers
{
    public class CollectiblesController : Controller
    {
        // GET: Collectibles
        public ActionResult Index()
        {
            return View();
        }
    }
}