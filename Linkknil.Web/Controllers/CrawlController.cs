using Linkknil.Jobs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Linkknil.Web.Controllers {
    public class CrawlController : Controller {
        //
        // GET: /Crawl/Run

        public ActionResult Run(bool sync = true) {
            var sw = Stopwatch.StartNew();
            var job = new CrawlJob();
            if (sync) {
                job.SyncDigLinks();
            }
            else {
                job.DigLinks();
            }
            sw.Stop();
            var unhandledLinks  =  job.GetUnhandledLinks(999, DateTime.Now.AddMinutes(30));
            var result = new {
                Span = sw.Elapsed.ToString() ,
                UnhandledLinks = unhandledLinks 
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
