using System.Web.Mvc;

namespace HelpDesk.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                  "Admin_Login",
                "Admin/Login",
                new
                {
                    area = "Admin",
                    controller = "Home",
                    action = "Login",
                    id = UrlParameter.Optional
                },
                new[] { "HelpDesk.Areas.Admin.Controllers" }
            );



            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}