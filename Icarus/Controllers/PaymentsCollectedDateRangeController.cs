﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Icarus.Models;
using Rotativa;

namespace Icarus.Controllers
{
    public class PaymentsCollectedDateRangeController : Controller
    {
        private ICARUSDBEntities db = new ICARUSDBEntities();

        // GET: AssertionSummaries
        [HttpGet]
        public ActionResult Index(DateTime? datefrom, DateTime? dateto)
        {
            if (Session["Username"] != null)
            {
                if (datefrom != null && dateto == null)
                {
                    string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                    var report = new ViewAsPdf(db.vrptPayments.ToList().Where(x => x.PaidDate == datefrom && x.IsVerified == "Y").OrderBy(x => x.PaidDate).ToList())
                    {
                        PageOrientation = Rotativa.Options.Orientation.Landscape,
                        PageSize = Rotativa.Options.Size.A4,
                        CustomSwitches = footer
                    };
                    return report;
                }
                else
                {
                    string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                    var report = new ViewAsPdf(db.vrptPayments.ToList().Where(x => x.PaidDate >= datefrom && x.PaidDate <= dateto && x.IsVerified == "Y").OrderBy(x => x.PaidDate).ToList())
                    {
                        PageOrientation = Rotativa.Options.Orientation.Landscape,
                        PageSize = Rotativa.Options.Size.A4,
                        CustomSwitches = footer
                    };
                    return report;
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