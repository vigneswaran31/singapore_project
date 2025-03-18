using HelpDeskEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpDeskBAL;
using HelpDeskEntity.Classes;
using HelpDesk.Classes;

namespace HelpDesk.Areas.Admin.Controllers
{
    public class SettingsController : AdminBaseController
    {
        #region Grid Methods

        #region Priorities

        //Get Operator Priorities list
        public ActionResult OptPriorities()
        {
            return View();
        }

        //Get Customer Priorities list
        public ActionResult CustPriorities()
        {
            return View();
        }

        //Get Priorities in Grid Format for Display in Grid.
        public string GetGridData(GridSettings grid,int UserType)
        {
            try
            {
                return new TicketPriorityBL().GetGridData(grid,UserType);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region Ticket Status

        //Get Ticket Status list
        public ActionResult TicketStatus()
        {
            return View();
        }

        //Get Ticket Status in Grid Format for Display in Grid.
        public string TicketStatusGetGridData(GridSettings grid)
        {
            try
            {
                return new TicketStatusBL().GetGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

       
        #region Support Categories

        //Get Support Categories list
        public ActionResult SupportCategories()
        {
            return View();
        }


        //Get Support Categories in Grid Format for Display in Grid.
        public string SupportCategoriesGetGridData(GridSettings grid)
        {
            try
            {
                return new SupportCategoryBL().GetGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion


        #endregion

        #region UI

        #region Priorities

        //Get operator Priority details by Id
        public ActionResult ManageOptPriority(int id)
        {
            TicketPriority oTicketPriority = null;

            if (id != 0)
            oTicketPriority = new TicketPriorityBL().GetById(id);

            return PartialView("_ManageOptPriority", oTicketPriority);
        }

        //Get Customer Priority details by Id
        public ActionResult ManageCusPriority(int id)
        {
            TicketPriority oTicketPriority = null;

            if (id != 0)
                oTicketPriority = new TicketPriorityBL().GetById(id);

            return PartialView("_ManageCusPriority", oTicketPriority);
        }
        
        #endregion

        #region Ticket Status

        //Get Ticket Status details by Id
        public ActionResult ManageTicketStatus(int id)
        {
            TicketStatu oTicketStatu = null;

            if (id != 0)
                oTicketStatu = new TicketStatusBL().GetById(id);

            return PartialView("_ManageTicketStatus", oTicketStatu);
        }


        #endregion

        #region Support Categories

        //Get Support Categories details by Id
        public ActionResult ManageSupportCategories(int id)
        {
            ViewBag.lstSprtCategories = new SupportCategoryBL().GetCategoryDDL(1);
            SupportCategory oSupportCategory = new SupportCategory();

            if (id != 0)
                oSupportCategory = new SupportCategoryBL().GetById(id);

            return PartialView("_ManageSupportCategories", oSupportCategory);
        }


        #endregion

        #endregion

        #region CRUD Methods

        #region Priorities

        // Create New or Update Existing Operator Priority in Db.
        public JsonResult SaveOptPriority(TicketPriority oTicketPriority)
        {
            try
            {
                int id = oTicketPriority.TicketPriorityId;
                if (id == 0)
                {
                    oTicketPriority.Type = Convert.ToInt16(En_Priority_Role.Operator);
                    new TicketPriorityBL().Create(oTicketPriority);
                }
                else
                    new TicketPriorityBL().Update(oTicketPriority);
                return Json(new { success = true, message = CommonMsg.Success(EntityNames.OprtrPriority, id == 0 ? En_CRUD.Insert : En_CRUD.Update) });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = CommonMsg.Fail(EntityNames.OprtrPriority, En_CRUD.Delete) });
            }
        }

        // Create New or Update Existing Customer Priority in Db.
        public JsonResult SaveCusPriority(TicketPriority oTicketPriority)
        {
            try
            {
                int id = oTicketPriority.TicketPriorityId;
                if (id == 0)
                {
                    oTicketPriority.Type = Convert.ToInt16(En_Priority_Role.Customer);
                    new TicketPriorityBL().Create(oTicketPriority);
                }
                else
                    new TicketPriorityBL().Update(oTicketPriority);
                return Json(new { success = true, message = CommonMsg.Success(EntityNames.CustomerPriority, id == 0 ? En_CRUD.Insert : En_CRUD.Update) });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = CommonMsg.Fail(EntityNames.CustomerPriority, En_CRUD.Delete) });
            }
        }

        //Delete Operator Priority by Id.
        public JsonResult DeleteOptPriority(int id)
        {
            try
            {
                new TicketPriorityBL().Delete(id);
                return Json(new { success = true, Message = CommonMsg.Success_Delete(EntityNames.OprtrPriority) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = CommonMsg.Fail_Delete(EntityNames.OprtrPriority) });
            }
        }

        //Delete Customer Priority by Id.
        public JsonResult DeleteCusPriority(int id)
        {
            try
            {
                new TicketPriorityBL().Delete(id);
                return Json(new { success = true, Message = CommonMsg.Success_Delete(EntityNames.CustomerPriority) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = CommonMsg.Fail_Delete(EntityNames.CustomerPriority) });
            }
        }
        #endregion

        #region Ticket Status

        // Create New or Update Existing Ticket Status in Db.
        public JsonResult SaveTicketStatus(TicketStatu oTicketStatu)
        {
            try
            {
                int id = oTicketStatu.TicketStatusId;
                if (id == 0)
                {
                    new TicketStatusBL().Create(oTicketStatu);
                }
                else
                    new TicketStatusBL().Update(oTicketStatu);
                return Json(new { success = true, message = CommonMsg.Success(EntityNames.TicketStatus, id == 0 ? En_CRUD.Insert : En_CRUD.Update) });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = CommonMsg.Fail(EntityNames.TicketStatus, En_CRUD.Delete) });
            }
        }

        //Delete Ticket Status by Id.
        public JsonResult DeleteTicketStatus(int id)
        {
            try
            {
                new TicketStatusBL().Delete(id);
                return Json(new { success = true, Message = CommonMsg.Success_Delete(EntityNames.TicketStatus) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = CommonMsg.Fail_Delete(EntityNames.TicketStatus) });
            }
        }

        #endregion

        #region Support Categories
        // Create New or Update Existing Support Categories in Db.
        public JsonResult SaveSupportCategories(SupportCategory oSupportCategory)
        {
            try
            {
                int id = oSupportCategory.CategoryId;
                if (id == 0)
                    new SupportCategoryBL().Create(oSupportCategory);
                else
                    new SupportCategoryBL().Update(oSupportCategory);

                return Json(new { success = true, message = CommonMsg.Success(EntityNames.SupportCategory, id == 0 ? En_CRUD.Insert : En_CRUD.Update) });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = CommonMsg.Fail(EntityNames.SupportCategory, En_CRUD.Delete) });
            }
        }

        //Delete Support Categories by Id.
        public JsonResult DeleteSupportCategories(int id)
        {
            try
            {
               new SupportCategoryBL().Delete(id);
               return Json(new { success = true, Message = CommonMsg.Success_Delete(EntityNames.SupportCategory) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = CommonMsg.DependancyError() });
            }
        }

        #endregion

        #endregion

    }
}