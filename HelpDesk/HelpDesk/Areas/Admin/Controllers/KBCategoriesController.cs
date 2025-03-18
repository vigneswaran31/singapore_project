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
    public class KBCategoriesController : AdminBaseController
    {
        #region Grid Methods

        //Get KB Categories list
        public ActionResult Index()
        {
            return View();
        }

        //Get KB Categories in Grid Format for Display in Grid.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                return new KBCategoryBL().GetGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region UI

        //Get KB Categories details by Id
        public ActionResult Manage(int id = 0)
        {
            try
            {
                ViewBag.lstCategories = new KBCategoryBL().GetCategoryDDL(1);

                KBCategory oCategory = new KBCategory();

                if (id > 0)
                    oCategory = new KBCategoryBL().GetById(id);

                return PartialView(oCategory);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Methods

        // Create New or Update Existing KB Category in Db.
        public JsonResult Save(KBCategory oCategory)
        {
            try
            {
                bool Add_Flg = new CommonBL().isNewEntry(oCategory.KBCategoryId);

                if (Add_Flg)
                    new KBCategoryBL().Create(oCategory);
                else
                    new KBCategoryBL().Update(oCategory);

                return Json(new { success = true, message = CommonMsg.Success(EntityNames.Category, Add_Flg == true ? En_CRUD.Insert : En_CRUD.Update) });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        //Delete KB Category
        public JsonResult Delete(int id = 0)
        {
            try
            {
                bool result = new KBCategoryBL().Delete(id);
                //Success
                if (result == true)
                    return Json(new { success = true, message = CommonMsg.Success(EntityNames.Category, En_CRUD.Delete) });
                else
                    return Json(new { success = false, message = CommonMsg.DependancyError() });
            }
            catch (Exception ex)
            {
                //Error
                return Json(new { success = false, message = CommonMsg.DependancyError() });
            }
        }

        #endregion
	}
}