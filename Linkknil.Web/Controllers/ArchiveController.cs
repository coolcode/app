using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CoolCode.Linq;
using CoolCode.Data.Entity;
using CoolCode.ServiceModel.Mvc;
using Linkknil.Entities;
using Linkknil.StreamStore;

namespace Linkknil.Web.Controllers {
    public class ArchiveController : SharedController<Linkknil.Models.LinkknilContext> {
        //
        // GET: /Archive/

        public ActionResult Index() {
            return View();
        }

        public ActionResult Details(string id) {
            var contentService = new AliyunFileService();
            var html = string.Empty;//
            try {
                html = contentService.GetContent(id);
            }
            catch (Exception ex){
                html = db.Contents.Where(c => c.Id == id).Select(c => c.FriendlyHtml).FirstOrDefault();
                Logger.Error(ex);
            }

            return this.Content(html, "text/html", Encoding.UTF8);
        }

        public ActionResult Css() {
            var path = Request.MapPath("~/Archives/style/readability.css");
            var css = System.IO.File.ReadAllText(path);

            return this.Content(css, "text/css");
        }

        public ActionResult Top() {
            var model = QueryContent(DateTime.Now);

            if (model.Any()) {
                ViewBag.LastIndex = model.Last().PublishTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            return View(model);
        }

        public ActionResult LazyLoad(DateTime? next) {
            string lastIndex = "1999-9-9";
            var list = QueryContent(next ?? DateTime.Parse(lastIndex));

            if (list.Any()) {
                lastIndex = list.Last().PublishTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            var model = new {
                Count = list.Count(),
                LastIndex = lastIndex,
                Items = list.ToArray(),
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<ContentViewModel> QueryContent(DateTime next) {
            return db.Query<ContentViewModel>(
                @"select top 30 c.Id, c.Title, substring(c.Text,1,100) Summary, c.EndTime as [PublishTime], a.Name as Author, a.IconPath 
from lnk_content c
left join pf_app a on c.AppId = a.Id
where c.EndTime < @LastTime
order by c.EndTime desc", new { LastTime = next });

        }
    }
}
