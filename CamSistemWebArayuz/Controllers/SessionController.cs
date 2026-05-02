using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CamSistemWebArayuz.Controllers
{
    class SessionController : ActionFilterAttribute
    {
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    if (!HttpContext.Current.User.Identity.IsAuthenticated)
        //    {
        //        if (!HttpContext.Current.Response.IsRequestBeingRedirected)
                    
        //    }
        //}

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {


            if (!HttpContext.Current.Request.IsAuthenticated || HttpContext.Current.Session["CurrentUser"] == null)
            {
                if (!filterContext.HttpContext.Response.IsRequestBeingRedirected)
                {

                    FormsAuthentication.SignOut();
                    filterContext.HttpContext.Response.Redirect("/Login/Login");
                }

            }
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();
            base.OnResultExecuting(filterContext);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}