﻿using System.Web;
using System.Web.Mvc;
using EaseEasy.Web.Mvc;

namespace Linkknil.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AjaxHandleErrorAttribute());
        }
    }
}