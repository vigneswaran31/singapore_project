using HelpDesk.Classes;
using HelpDesk.Models;
using HelpDeskBAL;
using HelpDeskEntity.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using HelpDeskEntity;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using HelpDeskEntity.Models;

namespace HelpDesk.Areas.Account.Controllers
{
    public class HomeController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            ViewBag.LoginFailed = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.LoginFailed);
            ViewBag.RememberMe = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.RememberMe);
            ViewBag.Login = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Login);
            ViewBag.IforgotMyPassword = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.IforgotMyPassword);
            ViewBag.SendMe = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.SendMe);
            ViewBag.BackToLogin = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.BackToLogin);
            ViewBag.RetrievePassword = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.RetrievePassword);
            ViewBag.PleaseEnterYourRegisteredEmail = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.PleaseEnterYourRegisteredEmail);

            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            base.Initialize(requestContext);
        }
        //Logout Current user from Applicaation
        public ActionResult Logout()
        {
            try
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    Session.Abandon();
                    FormsService.SignOut();
                }
                return Redirect("~/Home/Login");
            }
            catch (Exception ex)
            {
                return View(new { error = ex.Message });
            }
        }

        #region Change Password

        //Change Password for Requested Id.
        public ActionResult ChangePassword()
        {
            return View();
        }

        //Change password for User who requesting for this function.
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel oChangePasswordModel)
       {
            try
            {
                new UserBL().ChangePassword(oChangePasswordModel.NewPassword, (User)Session[En_UserSession.User.ToString()]);
                TempData["successmsg"] = "Your Password has been changed successfully.";
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = CommonMsg.Error();
            }
            return View();
        }



        #endregion

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        #endregion

    }
}