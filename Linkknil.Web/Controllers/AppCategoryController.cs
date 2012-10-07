using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using CoolCode.Linq;
using Linkknil.Entities;
using CoolCode.Web.Mvc;
using CoolCode.ServiceModel.Mvc;

namespace Linkknil.Web.Controllers {
    [Authorize]
    public class AppCategoryController : SharedController<Linkknil.Models.LinkknilContext> {
        #region Services


        #endregion

        public ActionResult Index() {
            //dynamic model = (from c in db.AppCategories select new { c.Id, c.Name, });

            return View();
        }

    }
}
