using System.Web.Mvc;
using EaseEasy.ServiceModel.Mvc;

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
