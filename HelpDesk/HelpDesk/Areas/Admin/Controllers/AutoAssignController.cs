using HelpDeskBAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpDeskEntity.Classes;
using HelpDeskEntity;
using HelpDesk.Classes;

namespace HelpDesk.Areas.Admin.Controllers
{
    public class AutoAssignController : AdminBaseController
    {
        //
        // GET: /Admin/AutoAssign/

        #region Grid Methods

        //Load AutoAssign Main Page
        public ActionResult Index()
        {
            return View();
        }

        //Load Company Page
        public ActionResult Company()
        {
            return PartialView("_Company");
        }

        //Load Category Page
        public ActionResult Category()
        {
            return PartialView("_Category");
        }

        //Load AutoAssign in Grid Format for Display in Grid.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                return new AutoAssignBL().GetGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //Load Company Data tickets in Grid Format for Display in Grid.
        public string CompanyGridData(GridSettings grid)
        {
            try
            {
                return new AutoAssignBL().CompanyGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //Load Category data in Grid Format for Display in Grid.
        public string CategoryGridData(GridSettings grid)
        {
            try
            {
                return new AutoAssignBL().CategoryGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region UI

        //Load Selected AutoAssign Data in Form
        public ActionResult ManageCompany(int id)
        {
            try
            {
                AutoAssign oAutoAssign = new AutoAssign();

                ViewBag.lstCompanies = new CompanyBL().GetAllEnableCompanies();
                ViewBag.lstOperators = new UserBL().GetAllByRole(En_Role.Operator.ToString());
                ViewBag.lstSupportTeam = new SupportTeamBL().GetAll();

                if (id > 0)
                    oAutoAssign = new AutoAssignBL().GetById(id);

                return PartialView("_ManageCompany", oAutoAssign);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Load Selected Category Data in Form
        public ActionResult ManageCategory(int id)
        {
            try
            {
                AutoAssign oAutoAssign = new AutoAssign();

                ViewBag.lstCategories = new SupportCategoryBL().GetCategoryDDL(1);
                ViewBag.lstOperators = new UserBL().GetAllByRole(En_Role.Operator.ToString());
                ViewBag.lstSupportTeam = new SupportTeamBL().GetAll();

                if (id > 0)
                    oAutoAssign = new AutoAssignBL().GetById(id);

                return PartialView("_ManageCategory", oAutoAssign);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Methods

        //Insert New or Update Existing AutoAssign
        public JsonResult Save(AutoAssign oAutoAssign)
        {
            try
            {
                AutoAssign obj = new AutoAssign();
                bool Add_Flg = new CommonBL().isNewEntry(oAutoAssign.Id);

                if (Add_Flg)
                {
                    if (oAutoAssign.CompanyId > 0)
                    {
                        if (oAutoAssign.OperatorId > 0)
                            obj = new AutoAssignBL().GetByCompanyAndUser(oAutoAssign.CompanyId.Value, oAutoAssign.OperatorId.Value);
                        else if (oAutoAssign.SupportTeamId > 0)
                            obj = new AutoAssignBL().GetByCompanyAndUser(oAutoAssign.CompanyId.Value, oAutoAssign.SupportTeamId.Value);
                    }
                    else if (oAutoAssign.CategoryId > 0)
                    {
                        if (oAutoAssign.OperatorId > 0)
                            obj = new AutoAssignBL().GetByCategoryAndUser(oAutoAssign.CategoryId.Value, oAutoAssign.OperatorId.Value);
                        else if (oAutoAssign.SupportTeamId > 0)
                            obj = new AutoAssignBL().GetByCategoryAndUser(oAutoAssign.CategoryId.Value, oAutoAssign.SupportTeamId.Value);
                    }

                    if (obj != null)
                        return Json(new { success = false, message = "CompanyId or CategoryId already exist.please select another" });
                }

                if (Add_Flg)
                    new AutoAssignBL().Create(oAutoAssign);
                else
                {
                    AutoAssign OldAutoAssign = new AutoAssignBL().GetById(oAutoAssign.Id);

                    if (OldAutoAssign.OperatorId > 0 && OldAutoAssign.SupportTeamId == null && oAutoAssign.SupportTeamId > 0)
                        oAutoAssign.OperatorId = null;
                    else if (OldAutoAssign.SupportTeamId > 0 && OldAutoAssign.OperatorId == null && oAutoAssign.OperatorId > 0)
                        oAutoAssign.SupportTeamId = null;

                    new AutoAssignBL().Update(oAutoAssign);
                }

                return Json(new { success = true, message = CommonMsg.Success(EntityNames.AutoAssign, Add_Flg == true ? En_CRUD.Insert : En_CRUD.Update) });
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        //Delete AutoAssign
        public JsonResult Delete(int id = 0)
        {
            try
            {
                new AutoAssignBL().Delete(id);
                return Json(new { success = true, message = CommonMsg.Success(EntityNames.AutoAssign, En_CRUD.Delete) });
            }
            catch (Exception ex)
            {
                //Error
                return Json(new { success = false, message = CommonMsg.Fail_Delete(EntityNames.AutoAssign) });
            }
        }

        #endregion
      
    }
}