using HelpDesk.Classes;
using HelpDesk.Models;
using HelpDeskBAL;
using HelpDeskEntity;
using HelpDeskEntity.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HelpDesk.Controllers
{
    public class HomeController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }

        private void LanguageHeader()
        {
            // Language Header
            ViewBag.LngWelcome = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Welcome);
            ViewBag.LngChangePassword = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ChangePassword);
            ViewBag.LngMyProfile = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.MyProfile);
            ViewBag.LngLogout = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Logout);
        }
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

            LanguageHeader();

            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            base.Initialize(requestContext);
        }

        public ActionResult Index()
        {                         
            return View("Login");
        }

        public ActionResult Login()
        {           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel oLoginViewModel, string returnUrl)
        {
            try
            {
                if (new UserBL().ValidateUser(oLoginViewModel.UserName, oLoginViewModel.Password))
                {
                    User oUser = new UserBL().GetByUserName(oLoginViewModel.UserName);

                    if (oUser.RoleId == (int)En_CompanyUserRole.CompanyUser || oUser.RoleId == (int)En_CompanyUserRole.CompanySuperUser
                         || oUser.RoleId == (int)En_Role.Operator)
                    {
                        FormsService.SignIn(oLoginViewModel.UserName, oLoginViewModel.RememberMe);
                        HelpDesk.Classes.SiteUtility.SetSessionVariables(oLoginViewModel.UserName);

                        if (oUser.RoleId == (int)En_Role.Operator)
                            return RedirectToAction("Index", "Dashboard", new { Area = "Account" });
                        else
                            return RedirectToAction("Index", "Dashboard");
                    }
                    else
                        ViewBag.Error = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.InvalidUsernamePassword) + ".";
                }
                else
                    ViewBag.Error = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.InvalidUsernamePassword) + ".";

                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
        

        #region Forgot Password Link

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendForgotPasswordLink(ForgotPasswordViewModel oForgotPasswordModel)
        {
            try
            {
                User oUser = new UserBL().GetByUserName(oForgotPasswordModel.Email);

                if (oUser != null)
                {
                    string token = Guid.NewGuid().ToString();
                    DateTime tokenExpirationDate = DateTime.Now.AddHours(Convert.ToInt32(ConfigurationManager.AppSettings["ForgotPasswordTokenExpirationHour"]));

                    oUser.VerificationToken = token;
                    oUser.VerificationTokenExpirationDate = tokenExpirationDate;

                    new UserBL().Update(oUser);

                    #region Send Email

                    EmailQueue oEmailQueue = new EmailQueue();

                    string link = Utility.CommonFunction.GetSite_URL() + "/Home/ResetPasswordFromLink?userId=" + oUser.UserId + "&token=" + token;

                    EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("ForgotPasswordWithLink",oUser.LanguageId.Value);

                    string body = oEmailTemplate.Body;
                    body = body.Replace("#username#", oUser.Email);
                    body = body.Replace("#link#", link);

                    oEmailQueue.To = oUser.Email;
                    oEmailQueue.ToName = oUser.Name;
                    oEmailQueue.Subject = oEmailTemplate.Subject;
                    oEmailQueue.Body = body;
                    oEmailQueue.CreatedOn = DateTime.Now;

                    EmailQueueBL.SendMail(oEmailQueue, true);

                    #endregion

                    TempData["successmsg"] = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.AMailWithReset_); 
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    TempData["errormsg"] = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.InvalidUsername) ;
                    return View("ForgotPassword");
                }
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = ex.Message;
                return View("ForgotPassword");
            }
        }

        [HttpGet]
        public ActionResult ResetPasswordFromLink(int userId, string token) //reset password from link validation
        {
            ResetPasswordViewModel oResetPasswordModel = new ResetPasswordViewModel();
            try
            {
                User oUser = new UserBL().GetById(userId);

                if (oUser != null && oUser.VerificationToken != null && oUser.VerificationToken == token)
                {
                    if (oUser.VerificationTokenExpirationDate >= DateTime.Now)
                    {
                        oResetPasswordModel.Email = oUser.Email;
                        return View("ResetPassword", oResetPasswordModel);
                    }
                    else
                    {
                        TempData["errormsg"] = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ResetPasswordLinkExpired_);
                        ViewBag.error = TempData["errormsg"];
                        return View("ResetPassword");
                    }
                }
                else
                {
                    TempData["errormsg"] = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ResetPasswordLinkNotValid);
                    ViewBag.error = TempData["errormsg"];
                    return View("ResetPassword");
                }
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = ex.Message;
                ViewBag.error = TempData["errormsg"];
                return View("ResetPassword", oResetPasswordModel);
            }
        }

        [HttpPost]
        public ActionResult SaveResetPassword(ResetPasswordViewModel oResetPasswordModel)
        {
            try
            {
                if (oResetPasswordModel.Email != null)
                {
                    User oUser = new UserBL().GetByUserName(oResetPasswordModel.Email);

                    if (oUser != null)
                    {
                        oUser.Password = oResetPasswordModel.Password;
                        new UserBL().Update(oUser);
                        TempData["successmsg"] = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.PasswordResetOk);
                        return RedirectToAction("Index", "Dashboard"); //redirect to dashboard
                    }
                    else
                    {
                        TempData["errormsg"] = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.PasswordResetError);
                        ViewBag.error = TempData["errormsg"];
                        return View("ResetPassword", oResetPasswordModel);
                    }
                }
                else
                {
                    TempData["errormsg"] = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.PasswordResetError);
                    ViewBag.error = TempData["errormsg"];
                    return View("ResetPassword", oResetPasswordModel);
                }
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = ex.Message;
                ViewBag.error = TempData["errormsg"];
                return View("ResetPassword", oResetPasswordModel);
            }
        }

        #endregion

        #region Change Password

        public ActionResult ChangePassword()
        {
            // Language Header
            ViewBag.LngWelcome = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Welcome);
            ViewBag.LngChangePassword = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ChangePassword);
            ViewBag.LngMyProfile = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.MyProfile);
            ViewBag.LngLogout = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Logout);

            ViewBag.LngCurrentPassword = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.CurrentPassword);
            ViewBag.LngNewPassword = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.NewPassword);
            ViewBag.LngConfirmPassword = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ConfirmPassword);
            ViewBag.LngSave = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Save);
            ViewBag.LngCancel = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Cancel);

            LanguageHeader();

            return View();
        }

        //Change password for User who requesting for this function.
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel oChangePasswordModel)
        {
            try
            {
                new UserBL().ChangePassword(oChangePasswordModel.NewPassword, (User)Session[En_UserSession.User.ToString()]);
                TempData["successmsg"] = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.PasswordResetOk);
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