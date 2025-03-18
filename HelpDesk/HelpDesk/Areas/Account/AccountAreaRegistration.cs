using System.Web.Mvc;

namespace HelpDesk.Areas.Account
{
    public class AccountAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Account";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                 "Account_Login",
               "Account/Login",
               new
               {
                   area = "Account",
                   controller = "Home",
                   action = "Login",
                   id = UrlParameter.Optional
               },
               new[] { "HelpDesk.Areas.Account.Controllers" }
           );

            context.MapRoute(
                "Account_default",
                "Account/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}