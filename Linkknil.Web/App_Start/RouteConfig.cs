using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Linkknil.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //p/guid"
            routes.MapRoute(
                name: "Archive",
                url: "p/{id}",
                defaults: new { controller = "Archive", action = "Details" }
            );
            //Archives/style/readability.css"
            routes.MapRoute(
                name: "Css",
                url: "p/style/readability.css",
                defaults: new { controller = "Archive", action = "Css" }
            );

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //首页
            routes.MapRoute(
                name: "HomeGroupbyApp",
                url: "apps/{appId}",
                defaults: new { controller = "Home", action = "Index", appId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "HomeGroupbyAppCategory",
                url: "category/{categoryId}",
                defaults: new { controller = "Home", action = "Index", categoryId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Cms", // Route name
                "{controller}-{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            /*f3b612/71afd5&text=enim*/
            routes.MapRoute(
               name: "Image_Format",
               url: "image/format/{width}x{height}/{id}",
               defaults: new { controller = "Image", action = "Format", width = 30, height = 30, id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "Image_Format_px",
               url: "image/format/{width}x{height}px/{id1}/{id2}_text={text}",
               defaults: new { controller = "Image", action = "Format", width = 30, height = 30, id1 = UrlParameter.Optional, id2 = UrlParameter.Optional,text=UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "Image_Format_common",
               url: "image/format/{width}x{height}/{id1}/{id2}",
               defaults: new { controller = "Image", action = "Format", width=30, height=30, id1 = UrlParameter.Optional, id2= UrlParameter.Optional }
           );
        }
    }
}