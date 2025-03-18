using HelpDesk.Classes;
using HelpDeskBAL;
using HelpDeskEntity;
using HelpDeskEntity.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpDesk.Areas.Admin.Controllers
{
    public class CompanyUsersController : AdminBaseController
    {
        //
        // GET: /Admin/CompanyUsers/

        #region Grid Methods

        public ActionResult Index()
        {
            ViewBag.ddlRole = new RoleBL().GetCompanyUserRoleFilter();
            return View();
        }

        //Load CompanyUsers in Grid Format for Display in Grid.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                User oUser = SiteUtility.GetCurrentUser();
                return new UserBL().GetGridData(grid, true, oUser.UserId);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region UI

        //Load Selected CompanyUsers Data in Form
        public ActionResult Manage(int id = 0)
        {
            User user = new User();
          
            ViewBag.lstCompanies = new CompanyBL().GetAllEnableCompanies();
            ViewBag.lstLanguages = new LanguageBL().GetAllLanguages();

            if (id > 0)
            {
                user = new UserBL().GetById(id);
            }
            return PartialView(user);
        }

        #endregion

        #region CRUD Methods

        //Create new or Update existing CompanyUsers.
        public JsonResult Save(User oUser)
        {
            try
            {
                bool Add_Flg = new CommonBL().isNewEntry(oUser.UserId);

                if (Request.Form["IsSuperUser"] == "on")
                    oUser.RoleId = (int)En_CompanyUserRole.CompanySuperUser;
                else
                    oUser.RoleId = (int)En_CompanyUserRole.CompanyUser;

                if (Add_Flg)
                    new UserBL().Create(oUser);
                else
                    new UserBL().Update(oUser);

                return Json(new { success = true, message = CommonMsg.Success(EntityNames.CompanyUser, Add_Flg == true ? En_CRUD.Insert : En_CRUD.Update) });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        #endregion

	}
}