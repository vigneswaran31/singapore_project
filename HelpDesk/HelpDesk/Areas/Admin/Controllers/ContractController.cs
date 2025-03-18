using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpDeskBAL;
using HelpDeskEntity.Classes;
using HelpDeskEntity;
using HelpDesk.Classes;


namespace HelpDesk.Areas.Admin.Controllers
{
    public class ContractController : AdminBaseController
    {
        #region Grid Methods

        #region ContractT emplate

        //Get Contract Template list.
        public ActionResult ContractTemplate()
        {
            return View();
        }

        //Load ContractTemplate in Grid Format for Display in Grid.
        public string ContractTemplateGetGridData(GridSettings grid)
        {
            try
            {
                return new ContractTemplateBL().GetGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region Company Contract

        //Get Company Contract list.
        public ActionResult CompanyContract()
        {
            return View();
        }

        //Load Company Contract in Grid Format for Display in Grid.
        public string CompanyContractGetGridData(GridSettings grid)
        {
            try
            {
                return new CompanyContractBL().GetGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #endregion

        #region UI

        #region ContractT emplate

        //Get ContractT emplate Details by Id
        public ActionResult ManageContractTemplate(int id)
        {
            ContractTemplate oContractTemplate = null;

            if (id != 0)
                oContractTemplate = new ContractTemplateBL().GetById(id);

            return PartialView("_ManageContractTemplate", oContractTemplate);
        }

        #endregion

        #region Company Contract

        //Get Company Contract Details by Id
        public ActionResult ManageCompanyContract(int id)
        {
            CompanyContract oCompanyContract = null;

            ViewBag.lstCompanies = new CompanyBL().GetAllEnableCompanies();
            ViewBag.lstTemplates = new ContractTemplateBL().GetAllEnableContractTemplates();

            if (id != 0)
                oCompanyContract = new CompanyContractBL().GetById(id);

            return PartialView("_ManageCompanyContract", oCompanyContract);
        }

        public ActionResult CopyCompanyContract(int id)
        {
            CompanyContract oCompanyContract = null;

            ViewBag.lstCompanies = new CompanyBL().GetAllEnableCompanies();
            ViewBag.lstTemplates = new ContractTemplateBL().GetAllEnableContractTemplates();

            oCompanyContract = new CompanyContractBL().GetById(id);
            oCompanyContract.StartDate = oCompanyContract.StartDate.AddYears(1);
            oCompanyContract.EndDate = oCompanyContract.EndDate.AddYears(1);
            oCompanyContract.CompanyContractId = 0;
            ViewBag.CopyCompanyContract = true;

            return PartialView("_ManageCompanyContract", oCompanyContract);
        }

        #endregion

        #endregion

        #region CRUD Methods

        #region ContractT emplate

        //Creat new or Update Existing Contract Template.
        public JsonResult SaveContractTemplate(ContractTemplate oContractTemplate)
        {
            try
            {
                int id = oContractTemplate.ContractTemplateId;
                if (id == 0)
                {
                    new ContractTemplateBL().Create(oContractTemplate);
                }
                else
                    new ContractTemplateBL().Update(oContractTemplate);
                return Json(new { success = true, message = CommonMsg.Success(EntityNames.ContractTemplate, id == 0 ? En_CRUD.Insert : En_CRUD.Update) });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = CommonMsg.Fail(EntityNames.ContractTemplate, En_CRUD.Delete) });
            }
        }

        //Delete Contract Template by Id.
        public JsonResult DeleteContractTemplate(int id)
        {
            try
            {
                new ContractTemplateBL().Delete(id);
                return Json(new { success = true, Message = CommonMsg.Success_Delete(EntityNames.ContractTemplate) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = CommonMsg.Fail_Delete(EntityNames.ContractTemplate) });
            }
        }

        #endregion

        #region Company Contract

        //Creat new or Update Existing Company Contract.
        public JsonResult SaveCompanyContract(CompanyContract oCompanyContract)
        {
            try
            {
                int id = oCompanyContract.CompanyContractId;
                if (id == 0)
                {
                    new CompanyContractBL().Create(oCompanyContract);
                }
                else
                    new CompanyContractBL().Update(oCompanyContract);
                return Json(new { success = true, message = CommonMsg.Success(EntityNames.CompanyContract, id == 0 ? En_CRUD.Insert : En_CRUD.Update) });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = CommonMsg.Fail(EntityNames.CompanyContract, En_CRUD.Delete) });
            }
        }

        //Delete Company Contract by Id.
        public JsonResult DeleteCompanyContract(int id)
        {
            try
            {
                new CompanyContractBL().Delete(id);
                return Json(new { success = true, Message = CommonMsg.Success_Delete(EntityNames.CompanyContract) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = CommonMsg.Fail_Delete(EntityNames.CompanyContract) });
            }
        }

        #endregion

        #endregion
    }
}