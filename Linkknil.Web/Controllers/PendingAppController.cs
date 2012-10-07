using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using CoolCode.Linq;
using Linkknil.Entities;
using CoolCode.Web.Mvc;
using CoolCode.ServiceModel.Mvc;

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
