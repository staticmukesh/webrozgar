using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebMatrix.WebData;
using WebRozgar.DAL;

namespace WebRozgar.Helpers
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        [Inject]
        public IWebRozgarService _Service { get; set; }

        public bool shouldbeused { get; set; }

        public CustomAuthorize()
        {
            shouldbeused = true;
        }
        public CustomAuthorize(bool value)
        {
            shouldbeused = value;
        }



        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!shouldbeused)
            {
                base.AuthorizeCore(httpContext);
            }

            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized)
            {
                return false;
            }
            string authenticateduser = httpContext.User.Identity.Name;

            if (!_Service.IsProfileCompleted(authenticateduser))
            {
                httpContext.Items["redirectToCompleteProfile"] = true;
                return false;
            }
            return true;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Items.Contains("redirectToCompleteProfile"))
            {
                var RouteValue = new RouteValueDictionary(new
                {
                    controller = "Account",
                    action = "manage",
                });
                filterContext.Result = new RedirectToRouteResult(RouteValue);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}