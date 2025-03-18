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
    public class EmailTemplateController : AdminBaseController
    {
        #region Grid Methods

        //Get EmailTemplate list.
        public ActionResult Index()
        {
            ViewBag.lstLanguages = new LanguageBL().GetAllLanguages();
            return View();
        }

        // get email template grid data
        public string GetGridData(GridSettings grid, int LangId)
        {
            try
            {
                if(LangId > 0)
                    return new EmailTemplateBL().GetGridData(grid,LangId);
                else
                    return new EmailTemplateBL().GetGridData(grid);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #endregion

        #region UI
        // email template view used for add n edit
        public ActionResult Manage(int id)
        {
            try
            {
                EmailTemplate oEmailTemplate = new EmailTemplate();
                ViewBag.lstLanguages = new LanguageBL().GetAllLanguages();

                if (TempData["oEmailTemplate"] != null) //if any error in save, resubmit old model
                {
                    ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
                    return View(TempData["oEmailTemplate"]);
                }
                if (id > 0)
                {
                    oEmailTemplate = new EmailTemplateBL().GetById(id);
                    ViewBag.lstTokens = new List<string>(oEmailTemplate.PredefinedTags.Split(',')); 
                }
                return View(oEmailTemplate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Operations

        // save email template record
        [ValidateInput(false)]
        public ActionResult Save(EmailTemplate oEmailTemplate)
        {
            bool Add_Flg = new CommonBL().isNewEntry(oEmailTemplate.EmailTemplateId);
            try
            {
                if (ModelState.IsValid)
                {
                    User user = (User)Session[En_UserSession.User.ToString()];
                    new EmailTemplateBL().Update(oEmailTemplate, user);

                    //Success
                    TempData["successmsg"] = CommonMsg.Success(EntityNames.EmailTemplate, Add_Flg ? En_CRUD.Insert : En_CRUD.Update);
                    return RedirectToAction("Index", "EmailTemplate");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", CommonMsg.Fail(EntityNames.EmailTemplate, Add_Flg ? En_CRUD.Insert : En_CRUD.Update));
            }
            TempData["ModelState"] = ModelState;
            TempData["oEmailTemplate"] = oEmailTemplate;
            return RedirectToAction("Manage", new { id = oEmailTemplate.EmailTemplateId });
        }

        //delete email template record
        public JsonResult Delete(int id = 0)
        {
            try
            {
                new EmailTemplateBL().Delete(id);
                return Json(new { success = true, message = CommonMsg.Success(EntityNames.EmailTemplate, En_CRUD.Delete) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Fail(EntityNames.EmailTemplate, En_CRUD.Delete) });
            }
        }

        #endregion
	}
}