using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CoolCode.ServiceModel.Mvc;

namespace Linkknil.Web.Controllers {
    public class ArchiveController : SharedController<Linkknil.Models.LinkknilContext> {
        //
        // GET: /Archive/

        public ActionResult Index() {
            return View();
        }

        public ActionResult Details(string id) {
            var html = db.Contents.Where(c => c.Id == id).Select(c => c.FriendlyHtml).FirstOrDefault();

            return this.Content(html, "text/html", Encoding.UTF8);
        }

        public ActionResult Css() {
            var path = Request.MapPath("~/Archives/style/readability.css");
            var css = System.IO.File.ReadAllText(path);

            return this.Content(css, "text/css");
        }
    }
}
