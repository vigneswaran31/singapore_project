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

namespace HelpDesk.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
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

                    if (oUser.RoleId == (int)En_Role.Admin)
                    {
                        FormsService.SignIn(oLoginViewModel.UserName, oLoginViewModel.RememberMe);
                        HelpDesk.Classes.SiteUtility.SetSessionVariables(oLoginViewModel.UserName);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                        ViewBag.Error = "Invalid Username or Password.";
                }
                else
                    ViewBag.Error = "Invalid Username or Password.";

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
                return Redirect("~/Admin/Home/Login");
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

                    string link = Utility.CommonFunction.GetSite_URL() + "/Admin/Home/ResetPasswordFromLink?userId=" + oUser.UserId + "&token=" + token;

                    EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("ForgotPasswordWithLink",1);

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

                    TempData["successmsg"] = "A mail with reset password link has been sent. Please check your Inbox.";
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    TempData["errormsg"] = "Invalid Username.";
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
                        TempData["errormsg"] = "Reset password link is expired, try reset password again.";
                        ViewBag.error = TempData["errormsg"];
                        return View("ResetPassword");
                    }
                }
                else
                {
                    TempData["errormsg"] = "Reset password link is not valid.";
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
                        TempData["successmsg"] = "Your password has been reset successfully.";
                        return RedirectToAction("Index", "Home"); //redirect to dashboard
                    }
                    else
                    {
                        TempData["errormsg"] = "Error resetting your Password. Please contact site Administrator.";
                        ViewBag.error = TempData["errormsg"];
                        return View("ResetPassword", oResetPasswordModel);
                    }
                }
                else
                {
                    TempData["errormsg"] = "Error resetting your Password. Please contact site Administrator.";
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