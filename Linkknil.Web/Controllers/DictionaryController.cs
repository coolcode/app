using System.Web.Mvc;
using EaseEasy.Linq;
using EaseEasy.ServiceModel.Services; 
using EaseEasy.Web.Mvc;
using Ninject;
using EaseEasy.ServiceModel.Mvc;

namespace Linkknil.Web.Controllers
{
	[Authorize]
	public class DictionaryController : SharedController
	{
		#region Services

		[Inject]
		public IDictionaryService DictionaryService { get; set; }

		#endregion

		//
		// GET: /Dictionary/

		public ActionResult Index()
		{
			return View();
		}

		public override ActionResult ItemList(string entity)
		{
			string name = this.ValueOf<string>("name");
			var qb = QueryBuilder.Create<DictionaryItem>();
			if (!string.IsNullOrEmpty(name))
			{
				qb.Equals(c => c.Name, name);
			}
			var model = QueryItems(qb);//this.SystemService.QueryDictionaryItem(qb);

			return View(model);
		}

		public ActionResult HeaderList()
		{
			var model = this.DictionaryService.ListDictionaryGroups();
			return View(model);
		}

        [HttpPost]
        public override ActionResult Edit(string entity, string id, FormCollection form)
        {
            //DictionaryItem model = new DictionaryItem();
            //TryUpdateModel(model);
            //var service = new EaseEasy.ServiceModel.Services.
            //this.SystemService.SaveDictionaryItem(model, this.UserID);

			return this.Success();
            return base.Edit(entity, id, form);
        }

		/*
		//
		// GET: /Dictionary/Edit
		public ActionResult Edit(int? id, string name)
		{
			DictionaryItem model;
			if (id.HasValue)
			{
				model = this.SystemService.GetDictionaryItem(id.Value);
			}
			else
			{
				model = new DictionaryItem
				{
					Name = name,
					Enabled = true
				};
			}
			return View(model);
		}

		//
		// POST: /Dictionary/Edit

		[HttpPost]
		public ActionResult Edit(FormCollection collection)
		{
			DictionaryItem model = new DictionaryItem();
			TryUpdateModel(model);
			this.SystemService.SaveDictionaryItem(model, this.UserID);

			return this.Success();
		}

		//
		// GET: /Dictionary/Delete/5

		public ActionResult Delete(int id)
		{
			this.SystemService.DeleteDictionaryItem(id);

			return this.Success();
		}
		*/
	}
}
