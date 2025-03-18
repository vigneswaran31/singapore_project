using HelpDesk.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpDeskEntity;
using HelpDeskBAL;
using HelpDeskEntity.Classes;

namespace HelpDesk.Controllers
{
    public class DashboardController : BaseController
    {
        private void LanguageHeader()
        {
            // Language Header
            ViewBag.LngWelcome = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Welcome);
            ViewBag.LngChangePassword = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ChangePassword);
            ViewBag.LngMyProfile = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.MyProfile);
            ViewBag.LngLogout = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Logout);
        }

        public ActionResult Index()
        {
            User oUser = SiteUtility.GetCurrentUser();
            ViewBag.TotalOpenTicket = new SupportTicketBL().GetUserDashboardCount(oUser);
            ViewBag.CompanyContract = new CompanyContractBL().GetActiveContractDetailByCompanyId(oUser.CompanyId.Value);

            //Language
            ViewBag.Company = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Company);
            ViewBag.ContractPeriod = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ContractPeriod);
            ViewBag.Unlimited = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Unlimited);
            ViewBag.NoOfTickets = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.NoOfTickets);
            ViewBag.TicketsLeft = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.TicketsLeft);
            ViewBag.ResponseWithin = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ResponseWithin);
            ViewBag.SolutionWithin = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.SolutionWithin);
            ViewBag.hours = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.hours);
            ViewBag.OpenTickets = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.OpenTickets);
            ViewBag.CreateSupportTicket = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.CreateSupportTicket);

            LanguageHeader();
            
            return View();
        }

        //Get Current User Profile
        public ActionResult MyProfile()
        {
            User oUser = SiteUtility.GetCurrentUser();

            //Language
            ViewBag.LngName = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Name);
            ViewBag.LngContact = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Contact);
            ViewBag.LngSave = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Save);
            ViewBag.LngCancel = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Cancel);

            LanguageHeader();

            return View(oUser);
        }

        #region CRUD Methods
        //Update User Profile
        public JsonResult UpdateProfile(User oUser)
        {
            try
            {
                oUser.UserId = SiteUtility.GetCurrentUser().UserId;
                oUser = new UserBL().UpdateProfile(oUser);
                HelpDesk.Classes.SiteUtility.SetSessionVariables(oUser.Email);
                return Json(new { success = true, message = CommonMsg.Success(EntityNames.Profile, En_CRUD.Update) });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Fail(EntityNames.Profile, En_CRUD.Update) });
            }
        }

        #endregion
	}
}