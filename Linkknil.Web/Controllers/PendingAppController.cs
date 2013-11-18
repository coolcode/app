using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using EaseEasy.Linq;
using Linkknil.Entities;
using EaseEasy.Web.Mvc;
using EaseEasy.ServiceModel.Mvc;

namespace Linkknil.Web.Controllers {
    [Authorize(Roles = "admin")]
    public class PendingAppController : SharedController<Linkknil.Models.LinkknilContext> {
        #region Services


        #endregion

        public ActionResult Index() {
            return View();
        }

    }
}
