using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using CoolCode.Web.Mvc;

namespace Linkknil.Web.Models {
    public static class HtmlHelperExtensions {
        public static IHtmlString DeleteLink(this HtmlHelper html, string action = "Delete", object routeValues = null) {
            var result = html.ImageActionLink("/Content/images/icons/cross.png", "删除", action, routeValues,
                                              new { confirm = "确认删除这项数据吗？", rel = "ajax" });
            return result;
        }

        public static IHtmlString AjaxLink(this HtmlHelper html, string linkText, string action, object routeValues = null) {
            var result = html.ActionLink(linkText, action, routeValues, new { rel = "ajax" });

            return result;
        }

        public static IHtmlString DialogLink(this HtmlHelper html, string linkText, string action, object routeValues = null) {
            var result = html.ActionLink(linkText, action, routeValues, new { @class = "iframe", rel = "modal" });

            return result;
        }
    }
}