using System.Web.Mvc;
using CoolCode.ServiceModel.Mvc;

namespace Linkknil.Web.Controllers {
    [Authorize]
    public class SystemController : SharedController {
        public ActionResult Index() {
            return View();
        }

        public ActionResult ItemList() {
            return View();
        }

    }
}
