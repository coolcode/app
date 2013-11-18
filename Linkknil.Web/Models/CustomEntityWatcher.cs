using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EaseEasy.ServiceModel.Services.Implement;

namespace Linkknil.Razor.Models {
	public class CustomEntityWatcher :DefaultEntityWatcher{
		public override  Type GetEntityType(string entityName) {
            Type entityType = Type.GetType("Linkknil.Entities." + entityName + ",Linkknil.Services");

			return entityType ?? base.GetEntityType(entityName);
		}
	}
}