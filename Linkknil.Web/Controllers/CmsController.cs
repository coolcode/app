using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using EaseEasy.Linq; 
using EaseEasy.ServiceModel.Mvc;

namespace Linkknil.Web.Controllers
{
	[Authorize]
	public class CmsController : SharedController
	{ 
		public ActionResult Index()
		{
			return View();
		}


		[HttpPost]
		[ValidateInput(false)]
		public override ActionResult Edit(string entity, string id, FormCollection form)
		{
			return base.Edit(entity, id, form);
		}
 
	}
}
