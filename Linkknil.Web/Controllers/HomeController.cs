using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CoolCode;
using CoolCode.Data.Entity;
using Linkknil.Entities;
using Linkknil.Web.Models;
using CoolCode.Web.Mvc;
using CoolCode.ServiceModel.Mvc;

namespace Linkknil.Web.Controllers {
    [HandleError]
    public class HomeController : SharedController {
        #region Services
        #endregion

        #region Home

        public ActionResult Index(int page=0)
        {
            var archives = QueryContent(page);

            return View(archives);
        }

        public ActionResult ArchiveList(int page=1) {
            var archive = new ArchiveController();

            return archive.Top();
        }

        private IPaginatedList<ContentViewModel> QueryContent(int page) {
            return db.Paging<ContentViewModel>(new PageParam(page,30),"PublishTime desc", 
                @"select c.Id, c.Title, substring(c.Text,1,100) Summary, c.EndTime as [PublishTime], a.Name as Author, a.IconPath, c.ImagePath 
from lnk_content c
left join pf_app a on c.AppId = a.Id");

        }

        public ActionResult Search() {
            return View();
        }

        public ActionResult About() {
            return View();
        }

        public ActionResult Help() {
            return View();
        }

        #endregion

        #region Login

        public ActionResult LogOn() {
            return View();
        }

        [HttpGet]
        public ActionResult ValidateUser(LogOnModel model, string returnUrl) {
            bool success = MembershipService.ValidateUser(model.UserName, model.Password);
            if (success) {
                FormsAuthenticationService.SignIn(model.UserName, model.RememberMe);
                if (string.IsNullOrEmpty(returnUrl)) {
                    //TODO:log who sign in the system...
                    var url = Request.UrlReferrer.Query;
                    var qs = HttpUtility.ParseQueryString(url);
                    returnUrl = qs["ReturnUrl"];
                }

                return this.Success(new { url = returnUrl });
            } else {
                return this.Fail("用户名或密码错误！");
            }
        }

        public ActionResult LogOff() {
            FormsAuthenticationService.SignOut();
            return RedirectToAction("LogOn");
        }

        #endregion

        #region Register

        public ActionResult Register() {
            initial_Register();

            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model) {
            if (ModelState.IsValid) {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success) {
                    FormsAuthenticationService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                } else {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            initial_Register();
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void initial_Register() {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus) {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus) {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
    }
}
