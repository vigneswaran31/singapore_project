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
    public class MailController : Controller
    {
        #region Grid Methods

        public ActionResult BlackList()
        {
            return View();
        }

        // Get Blacklist grid data
        public string GetBlackListGridData(GridSettings grid)
        {
            try
            {
                return new EmailBlackListBL().GetGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public ActionResult Incoming()
        {
            return View();
        }

        public ActionResult UnProcessedEmailInbox()
        {
            return View();
        }

        // Get Incoming Grid data
        public string GetIncomingGridData(GridSettings grid)
        {
            try
            {
                return new EmailInboxBL().GetGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Get Unprocessed Grid data
        public string GetUnProcessedEmailInboxGridData(GridSettings grid)
        {
            try
            {
                return new EmailInboxBL().GetUnProcessedGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region UI

        public ActionResult ManageBlackList(int id = 0)
        {
            EmailBlackList oEmailBlackList = new EmailBlackList();
            if (id > 0)
                oEmailBlackList = new EmailBlackListBL().GetById(id);

            return PartialView("_ManageBlackList", oEmailBlackList);
        }

        #endregion

        #region CRUD Methods

        public JsonResult SaveBlackList(EmailBlackList oEmailBlackList)
        {
            try
            {
                bool Add_Flg = new CommonBL().isNewEntry(oEmailBlackList.Id);

                if (Add_Flg)
                    new EmailBlackListBL().Create(oEmailBlackList);
                else
                    new EmailBlackListBL().Update(oEmailBlackList);

                return Json(new { success = true, message = CommonMsg.Success(EntityNames.EmailBlackList, Add_Flg == true ? En_CRUD.Insert : En_CRUD.Update) });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        public JsonResult DeleteBlackList(int id)
        {
            try
            {
                bool result = new EmailBlackListBL().Delete(id);

                if (result == true)
                    return Json(new { success = true, message = CommonMsg.Success(EntityNames.EmailBlackList, En_CRUD.Delete) });
                else
                    return Json(new { success = false, message = CommonMsg.DependancyError() });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.DependancyError() });
            }
        }

        #endregion

        #region Incoming

        // Add record in Black list
        public JsonResult AddToBlacklist(int id)
        {
            try
            {
                EmailInbox oEmailInbox = new EmailInboxBL().GetById(id);
                EmailBlackList oEmailBlackList = new EmailBlackList();
                oEmailBlackList.MailAddress = oEmailInbox.FromAddress;
                oEmailBlackList.DeleteOnRead = false;
                new EmailBlackListBL().Create(oEmailBlackList);
                new EmailInboxBL().Delete(id);

                return Json(new { success = true, message = "Email has been successfully added to the Email Blacklist table" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        // Create New Ticket
        public JsonResult CreateTicket(int id)
        {
            try
            {
                EmailInbox oEmailInbox = new EmailInboxBL().GetById(id);
                User oUser = new UserBL().GetById(oEmailInbox.UserId.Value);

                vw_CompanyContract Contract = new CompanyContractBL().GetActiveContractDetailByCompanyId(oUser.CompanyId.Value);

                if (Contract == null)
                    return Json(new { success = false, message = "Ticket creation fails because Company User has no active Contracts." });

                Ticket oTicket = new Ticket();
                oTicket.TicketGuid = Guid.NewGuid();
                oTicket.Title = oEmailInbox.Subject;
                oTicket.ProblemDescription = oEmailInbox.Body;
                oTicket.CategoryId = new SupportCategoryBL().GetDefaultSupportCategory().CategoryId;
                oTicket.SourceId = new TicketsSourcesBL().GetDefaultTicketSourceForMail().Id;

                oTicket.CurrentStatus = new TicketStatusBL().GetDefaultStatusForNewTicket().TicketStatusId;
                oTicket.OperatorPriority = new TicketPriorityBL().GetDefaultPriorityByType(Convert.ToInt16(En_Priority_Role.Operator)).TicketPriorityId;
                oTicket.CustomerPriority = new TicketPriorityBL().GetDefaultPriorityByType(Convert.ToInt16(En_Priority_Role.Customer)).TicketPriorityId;
                oTicket.ContractId = new CompanyContractBL().GetActiveContractDetailByCompanyId(oUser.CompanyId.Value).CompanyContractId;
                oTicket.CompanyUserId = oEmailInbox.UserId.Value;
                oTicket.CompanyId = oUser.CompanyId.Value;

                new SupportTicketBL().Create(oTicket);

                oEmailInbox.LinkedToTicketId = oTicket.TicketViewId;
                new EmailInboxBL().Update(oEmailInbox);

                return Json(new { success = true, message = CommonMsg.Success_Insert(EntityNames.SupportTicket), TicketId = oTicket.TicketViewId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        public ActionResult AssignToUser(int id, string TicketId)
        {
            ViewBag.LstOperators = new UserBL().GetAllByRole(En_Role.Operator.ToString());
            Ticket oTicket = new SupportTicketBL().GetByTicketViewId(TicketId);
            ViewBag.OperatorId = new SupportTicketBL().GetById(oTicket.TicketId).AssignToOperator;
            return PartialView("_AssignToUser", id);
        }

        // save Assign Ticket To Operator
        public JsonResult SaveAssignOperator(int OperatorId, string TicketId)
        {
            try
            {
                Ticket oTicket = new SupportTicketBL().GetByTicketViewId(TicketId);
                new SupportTicketBL().AssignOperatorToTicket(oTicket.TicketId, OperatorId);
                return Json(new { success = true, message = "Operator has been successfully assigned to Ticket." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        public ActionResult AssignToTicket(int Id)
        {
            ViewBag.lstOpenTickets = from ticket in new SupportTicketBL().GetAllOpenTickets()
                                     select new SelectListItem
                                     {
                                         Text = ticket.TicketViewId + " ( " + ticket.Title + " )",
                                         Value = ticket.TicketViewId.ToString(),
                                     };

            return PartialView("_AssignToTicket", Id);
        }

        // Assign Ticket to LinkedToTicketId
        public JsonResult SaveAssignTicket(string TicketId, int InboxId)
        {
            try
            {
                EmailInbox oEmailInbox = new EmailInboxBL().GetById(InboxId);
                oEmailInbox.LinkedToTicketId = TicketId;
                new EmailInboxBL().Update(oEmailInbox);
                return Json(new { success = true, message = "Ticket has been assigned successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        // update processed = true
        public JsonResult Processed(int Id)
        {
            try
            {
                EmailInbox oEmailInbox = new EmailInboxBL().GetById(Id);
                oEmailInbox.Processed = true;
                new EmailInboxBL().Update(oEmailInbox);
                return Json(new { success = true, message = "EmailInbox has been Processed successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        #endregion
    }
}