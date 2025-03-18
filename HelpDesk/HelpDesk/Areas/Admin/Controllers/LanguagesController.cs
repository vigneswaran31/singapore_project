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
    public class LanguagesController : Controller
    {
        //
        // GET: /Admin/Languages/
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
                return new LanguageBL().GetGridData(grid);
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
                Language oLanguage = new Language();
                if (id > 0)
                    oLanguage = new LanguageBL().GetById(id);
                return PartialView("_Manage",oLanguage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Methods

        public JsonResult Save(Language oLanguage)
        {
            try
            {
                bool Add_Flg = new CommonBL().isNewEntry(oLanguage.Id);

                if (Add_Flg)
                    new LanguageBL().Create(oLanguage);
                else
                    new LanguageBL().Update(oLanguage);

                return Json(new { success = true, message = CommonMsg.Success(EntityNames.Language, Add_Flg == true ? En_CRUD.Insert : En_CRUD.Update) });
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
                bool result = new LanguageBL().Delete(id);

                if (result == true)
                    return Json(new { success = true, message = CommonMsg.Success(EntityNames.Language, En_CRUD.Delete) });
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