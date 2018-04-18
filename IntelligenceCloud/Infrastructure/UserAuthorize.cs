using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntelligenceCloud.Infrastructure
{
    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        public string AuthorizationFailView { get; set; }
        private bool featureAccess;
        //請求授權時執行
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            featureAccess = false;
            //獲得url請求裡的controller和action
            string controllerName = filterContext.RouteData.Values["controller"].ToString();

            base.OnAuthorization(filterContext);//進入AuthorizeCore
        }

        //自定義授權檢查 (return false 則禁止存取頁面)
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                featureAccess = httpContext.User.IsInRole("admin");

            }
            //return base.AuthorizeCore(httpContext);
            return featureAccess;
            //return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            string controllerName = "Home";
            if (controllerName != null)
            {
                //導到別頁
                filterContext.HttpContext.Response.RedirectToRoute(new { controller = controllerName, action = "Index" });
            }
            else
            {
                //用URL
                filterContext.HttpContext.Response.Redirect("~/Home/Logout");
            }


            //filterContext.Result = new ViewResult { ViewName = AuthorizationFailView };
        }
    }
}