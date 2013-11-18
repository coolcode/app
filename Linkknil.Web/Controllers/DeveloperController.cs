using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using EaseEasy;
using EaseEasy.Linq;
using Linkknil.Entities;
using EaseEasy.Web.Mvc;
using EaseEasy.ServiceModel.Mvc;

namespace Linkknil.Web.Controllers {
    [Authorize]
    public class DeveloperController : SharedController<Linkknil.Models.LinkknilContext> {
        #region Services


        #endregion

        [HttpGet]
        public ActionResult CreateApp() {
            ViewBag.AppCategories = db.AppCategories
                .Where(c => c.Status == (int)ItemStatus.Enabled)
                .ToList()
                .Select(c => new ValueText(c.Id, c.Name));

            return View();
        }

        [HttpPost]
        public ActionResult CreateApp(FormCollection form) {
            var app = new App();
            TryUpdateModel(app);

            app.Id = Guid.NewGuid().ToString();
            app.CreateTime = DateTime.Now;
            app.DeveloperId = UserID;
            app.Status = (int)AppStatus.Draft;

            db.Apps.Add(app);
            db.SaveChanges();

            return this.Success();
        }

        [HttpPost]
        public ActionResult EditApp(string id, FormCollection form) {
            var app = db.Apps.Find(id);
            TryUpdateModel(app);

            app.UpdateTime = DateTime.Now;

            db.Entry(app).State = EntityState.Modified;
            db.SaveChanges();

            return this.Success();
        }

        public ActionResult SubmitApp(string id) {
            var app = db.Apps.Find(id);
            app.Status = (int)AppStatus.Pending;

            db.Entry(app).State = EntityState.Modified;
            db.SaveChanges();

            return this.Success();
        }

        public ActionResult OffApp(string id) {
            var app = db.Apps.Find(id);
            app.Status = (int) AppStatus.Offline;

            db.Entry(app).State = EntityState.Modified;
            db.SaveChanges();

            return this.Success();
        }

        public ActionResult AppList(int page = 0) {
            var q = from c in db.Apps
                    where c.DeveloperId == UserID
                    orderby c.CreateTime descending
                    select c;

            var model = q.Paging(new PageParam(page));

            return View(model);
        }

        public ActionResult AppDetails(string id) {
            var app = db.Apps.Find(id);
            var links = db.Links.Where(c => c.AppId == id).ToList();
            ViewBag.App = app;
            ViewBag.Links = links;

            return View();
        }

        [HttpGet]
        public ActionResult CreateLink(string appId) {
            ViewBag.App = db.Apps.Find(appId);
            return View("EditLink");
        }

        [HttpPost]
        public ActionResult CreateLink(string appId, FormCollection form) {
            var model = new LinkItem();
            TryUpdateModel(model);
            model.Id = Guid.NewGuid().ToString();
            model.CreateTime = DateTime.Now;

            UpdateAppStatusWhenLinkItemChanged(appId);

            db.Links.Add(model);
            db.SaveChanges();

            return this.Success();
        }

        [HttpGet]
        public ActionResult EditLink(string id, string appId) {
            ViewBag.App = db.Apps.Find(appId);
            var model = db.Links.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditLink(string appId, FormCollection form) {
            var model = new LinkItem();
            TryUpdateModel(model);
            model.UpdateTime = DateTime.Now;

            UpdateAppStatusWhenLinkItemChanged(appId);
            db.Entry(model).State = EntityState.Modified;

            db.SaveChanges();
            return this.Success();
        }

        //编辑内容源需要重新审核
        private void UpdateAppStatusWhenLinkItemChanged(string appId) {
            var app = db.Apps.Find(appId);

            if (app.Status == (int)AppStatus.Publish) {
                app.Status = (int)AppStatus.Pending;

            }
        }
    }
}
