using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CoolCode.Linq;
using Linkknil.Entities;
using CoolCode.Web.Mvc;
using CoolCode.ServiceModel.Mvc;
using Linkknil.Services;

namespace Linkknil.Web.Controllers {
    [Authorize]
    public class AppController : SharedController<Linkknil.Models.LinkknilContext> {
        #region Services


        #endregion

        public ActionResult PendingIndex() {
            ViewBag.Title = "待审核应用";
            ViewBag.ItemListName = "PendingList";
            return View("Index");
        }

        public ActionResult PublishIndex() {
            ViewBag.Title = "上线应用";
            ViewBag.ItemListName = "PublishList";
            return View("Index");
        }

        public ActionResult RejectIndex() {
            ViewBag.Title = "审核不通过应用";
            ViewBag.ItemListName = "RejectList";
            return View("Index");
        }

        public ActionResult PendingList() {
            return View(QueryApp(AppStatus.Pending));
        }

        public ActionResult PublishList() {
            return View(QueryApp(AppStatus.Publish));
        }

        public ActionResult RejectList() {
            return View(QueryApp(AppStatus.Reject));
        }

        private IQueryable<AppViewModel> QueryApp(AppStatus status) {
            var q = from c in db.Apps
                    where c.Status == (int)status
                    select new AppViewModel {
                        Id = c.Id,
                        Dev = c.DeveloperId,
                        Name = c.Name,
                        CreateTime = c.CreateTime,
                        AuditTime = c.AuditTime,
                        UpdateTime = c.UpdateTime,
                        PublishTime = c.PublishTime,
                    };
            var model = QueryItems(q);
            return model;
        }

        public ActionResult Details(string id) {
            var item = db.Apps.Find(id);
            if (item == null) {
                return this.Error("找不到应用");
            }

            return View(item);
        }

        public ActionResult PendingDetails(string id) {
            var item = db.Apps.Find(id);
            if (item == null) {
                return this.Error("找不到应用");
            }

            return View(item);
        }

        public ActionResult Pass(string id, string suggestion) {
            var model = db.Apps.Find(id);
            model.Status = (int)AppStatus.Publish;
            model.AuditorId = UserID;
            model.AuditTime = DateTime.Now;
            model.PublishTime = DateTime.Now;
            model.Suggestion = suggestion;
            db.SaveChanges();

            return this.Success();
        }

        public ActionResult NotPass(string id, string suggestion) {
            var model = db.Apps.Find(id);
            model.Status = (int)AppStatus.Reject;
            model.AuditorId = UserID;
            model.AuditTime = DateTime.Now;
            model.Suggestion = suggestion;
            db.SaveChanges();

            return this.Success();
        }

        public ActionResult ContentIndex() {
            return View();
        }

        public ActionResult ContentList() {
            var q = from c in db.Contents
                    select new { c.Id, c.Url, c.Title, c.Text, c.BeginTime, c.EndTime, c.TimeSpan };

            q = QueryItems(q);

            return View(q);
        }


        public ActionResult Pull(string id)
        {
            var linkService = new LinkService();
            var link = db.Links.Find(id);
                linkService.DigLink(link);

            return this.Success("抓取成功！");
        }
    }
}
