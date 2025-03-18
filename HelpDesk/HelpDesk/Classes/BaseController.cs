using HelpDesk.Models;
using HelpDeskEntity;
using HelpDeskEntity.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace HelpDesk.Classes
{
    public class BaseController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            base.Initialize(requestContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            var ControllerName = filterContext.Controller.GetType().Name.ToLower();
            var ActionName = filterContext.ActionDescriptor.ActionName.ToLower();
            if (HttpContext.User == null || HttpContext.User.Identity.IsAuthenticated == false || 
                (((User)Session[En_UserSession.User.ToString()]).RoleId != (int)En_Role.Operator &&
                ((User)Session[En_UserSession.User.ToString()]).RoleId != (int)En_CompanyUserRole.CompanyUser &&
                ((User)Session[En_UserSession.User.ToString()]).RoleId != (int)En_CompanyUserRole.CompanySuperUser
                ))
            {
                //if ajax request
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 401;
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            Error = "NotAuthorized",
                            LogOnUrl = FormsAuthentication.LoginUrl
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                    filterContext.HttpContext.Response.End();
                    return;
                }
                //not ajax request
                filterContext.Result = new RedirectResult("/Home/Index?returnUrl=" + filterContext.HttpContext.Request.Url.PathAndQuery);
                return;
            }


            base.OnActionExecuting(filterContext);
        }
	}
}