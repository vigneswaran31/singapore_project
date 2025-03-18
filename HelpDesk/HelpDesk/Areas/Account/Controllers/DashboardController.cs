using HelpDesk.Classes;
using HelpDeskEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpDeskBAL;
using HelpDeskEntity.Classes;

namespace HelpDesk.Areas.Account.Controllers
{
    public class DashboardController : BaseController
    {
        //
        // GET: /Account/Dashboard/
        public ActionResult Index()
        {
            OperatorDashboardCount oDasboardCountModel = new OperatorDashboardCount();
            User oUser = SiteUtility.GetCurrentUser();
            oDasboardCountModel = new SupportTicketBL().GetOperatorDashboardCount(oUser.UserId);
            return View(oDasboardCountModel);
        }

        //Get Current User Profile
        public ActionResult MyProfile()
        {
            User oUser = SiteUtility.GetCurrentUser();
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
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion



    }
}