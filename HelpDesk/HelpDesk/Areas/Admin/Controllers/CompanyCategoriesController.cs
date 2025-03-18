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
    public class CompanyCategoriesController : Controller
    {
        //
        // GET: /Admin/CompanyCategories/
        #region Grid Methods
        public ActionResult Index()
        {
            return View();
        }

        //Get TicketSource in Grid Format for Display in Grid.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                return new CompanyCategoryBL().GetGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region UI

        public ActionResult Manage(int id = 0)
        {
            try
            {
                CompanyCategory oCompanyCategory = new CompanyCategory();
                if (id > 0)
                    oCompanyCategory = new CompanyCategoryBL().GetById(id);
                return PartialView("_Manage", oCompanyCategory);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Methods

        public JsonResult Save(CompanyCategory oCompanyCategory)
        {
            try
            {
                bool Add_Flg = new CommonBL().isNewEntry(oCompanyCategory.Id);

                if (Add_Flg)
                    new CompanyCategoryBL().Create(oCompanyCategory);
                else
                    new CompanyCategoryBL().Update(oCompanyCategory);

                return Json(new { success = true, message = CommonMsg.Success(EntityNames.CompanyCategory, Add_Flg == true ? En_CRUD.Insert : En_CRUD.Update) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        public JsonResult Delete(int id)
        {
            try
            {
                bool result = new CompanyCategoryBL().Delete(id);

                if (result == true)
                    return Json(new { success = true, message = CommonMsg.Success(EntityNames.CompanyCategory, En_CRUD.Delete) });
                else
                    return Json(new { success = false, message = CommonMsg.DependancyError() });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        #endregion
	}
}