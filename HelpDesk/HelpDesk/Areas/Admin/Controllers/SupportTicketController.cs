using HelpDesk.Classes;
using HelpDeskBAL;
using HelpDeskEntity;
using HelpDeskEntity.Classes;
using HelpDeskEntity.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace HelpDesk.Areas.Admin.Controllers
{
    public class SupportTicketController : AdminBaseController
    {
        //
        // GET: /Admin/SupportTicket/

        #region Grid Methods

        //Load Support Ticket Main Page
        public ActionResult Index()
        {
            ViewBag.ddlStatus = GetStatus();
            ViewBag.ddlOptPriorities = GetOperatorPriority();
            ViewBag.ddlCusPriorities = GetCustomerPriority();
            ViewBag.ddlCategories = GetCategories();

            return View();
        }

        //Load Support tickets in Grid Format for Display in Grid.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                User oUser = SiteUtility.GetCurrentUser();
                return new SupportTicketBL().GetAllTickets(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Advance search order grid data
        public string GetAdvSearchResult(GridSettings grid, SearchTicketModel AdvSearch)
        {
            try
            {
                User oUser = SiteUtility.GetCurrentUser();
                return new SupportTicketBL().GetAllTickets(grid, 0, AdvSearch);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        //Load Events in Grid Format for Display in Grid.
        public string GetEventGridData(GridSettings grid, Guid id)
        {
            try
            {
                return new EventBL().GetGridData(grid, id);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        // Get Phone calls grid data
        public string GetPhoneCallsGridData(GridSettings grid, int Id)
        {
            try
            {
              return new PhoneCallsBL().GetGridDataByTicketId(grid,Id);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Get Activity grid data
        public string GetActivityGridData(GridSettings grid, int Id)
        {
            try
            {
                return new TicketActivityBL().GetGridData(grid, Id);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Get Mails grid data
        public string GetMailsGridData(GridSettings grid, int Id)
        {
            try
            {
                Ticket oTicket = new SupportTicketBL().GetById(Id);
                return new EmailInboxBL().GetGridData(grid, oTicket.TicketViewId);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region UI

        //Load Tab's Page for Selected Support Ticket.
        public ActionResult View_Detail(Guid? id = null, string tab = "")
        {
            ViewBag.tab = tab;

            if (id != null)
            {
                vw_SupportTicket ovw_SupportTicket = new SupportTicketBL().GetTicketViewDetailByGuid(id.Value);
                ViewBag.TicketId = ovw_SupportTicket.TicketViewId;
                ViewBag.CreatedOn = ovw_SupportTicket.CreatedOn;
                ViewBag.ClosedOn = ovw_SupportTicket.ClosedOn;
                if (new ContractTemplateBL().IsSlaAllowOnContractById(ovw_SupportTicket.ContractId))
                {
                    if (ovw_SupportTicket.ResponseStatus == "ResDone" && ovw_SupportTicket.SolutionStatus == "SolDone")
                    {
                        ViewBag.NextActionType = "None";
                        ViewBag.NextActionDueFor = "None";
                    }
                    else if (ovw_SupportTicket.ResponseStatus == "ResPending")
                    {
                        ViewBag.NextActionType = "Response";
                        ViewBag.NextActionDueFor = ovw_SupportTicket.ResponseDeadline.ToString();
                    }
                    else if (ovw_SupportTicket.SolutionStatus == "SolPending")
                    {
                        ViewBag.NextActionType = "Solution";
                        ViewBag.NextActionDueFor = ovw_SupportTicket.SolutionDeadline.ToString();
                    }
                    else if (ovw_SupportTicket.ResponseStatus == "ResOutofSLA" && ovw_SupportTicket.SolutionStatus == "SolOutofSLA")
                    {
                        ViewBag.NextActionType = "Out of SLA";
                        ViewBag.NextActionDueFor = "Out of SLA";
                    }
                }

            }

            return View(id);
        }

        //Load New Support Ticket Form.
        public ActionResult Create()
        {
            Ticket oTicket = new Ticket();
            //lookups
            ViewBag.OperatorPriorities = new TicketPriorityBL().GetPrioritiesByType(Convert.ToInt16(En_Priority_Role.Operator));
            ViewBag.CustomerPriorities = new TicketPriorityBL().GetPrioritiesByType(Convert.ToInt16(En_Priority_Role.Customer));
            ViewBag.LstOperators = new UserBL().GetAllByRole(En_Role.Operator.ToString());
            ViewBag.LstSupportTeams = new SupportTeamBL().GetAll();
            ViewBag.CompanyUsers = GetCompanyUsers();
            //permissions
            oTicket.CurrentStatus = new TicketStatusBL().GetDefaultStatusForNewTicket().TicketStatusId;
            oTicket.OperatorPriority = new TicketPriorityBL().GetDefaultPriorityByType(Convert.ToInt16(En_Priority_Role.Operator)).TicketPriorityId;
            oTicket.CustomerPriority = new TicketPriorityBL().GetDefaultPriorityByType(Convert.ToInt16(En_Priority_Role.Customer)).TicketPriorityId;
            //Attachments
            List<Attachment> attachments = new AttachmentsBL().GetByTypeAndId((int)En_LinkType.SupportTicket, oTicket.TicketId);
            ViewBag.Attachments = JsonConvert.SerializeObject(attachments);

            ViewBag.lstTicketsSource = from ticket in  new TicketsSourcesBL().GetAllTicketSources()
                                     select new SelectListItem
                                     {
                                            Text = ticket.Description,
                                            Value = ticket.Id.ToString(),
                                     };
            ViewBag.DefaultForTicket = new TicketsSourcesBL().GetDefaultTicketSourceForTicket().Id;

            return View(oTicket);
        }

        //Load Advance search pan+el.
        public ActionResult AdvancedSearch()
        {
            ViewBag.ddlStatus = new TicketStatusBL().GetStatus();
            ViewBag.ddlCompanies = new CompanyBL().GetAllEnableCompanies();
            ViewBag.ddlOptPriorities = new TicketPriorityBL().GetPrioritiesByType(Convert.ToInt16(En_Priority_Role.Operator));
            ViewBag.ddlCusPriorities = new TicketPriorityBL().GetPrioritiesByType(Convert.ToInt16(En_Priority_Role.Customer));
            ViewBag.ddlAssgnedToOperators = new UserBL().GetAllByRole(En_Role.Operator.ToString());
            ViewBag.ddlAssgnedToSupportTeams = new SupportTeamBL().GetAll();

            List<sp_GetSupportCategoryList_Result> lstCategories = new SupportCategoryBL().GetCategoryDDL(2);
            ViewBag.ddlCategories = from Category in lstCategories
                                    select new SelectListItem
                                    {
                                        Text = Category.CategoryName.ToString(),
                                        Value = Category.CategoryId.ToString()
                                    };


            return PartialView("_AdvancedSearch");
        }

        //Get All Attachments for Selected SupportTicket
        public ActionResult Attachments(int id)
        {
            Ticket oTicket = new SupportTicketBL().GetById(id);
            List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
            ViewBag.IsTicket_Open = lstOpenStatusId.Contains(oTicket.CurrentStatus) ? true : false;
            List<Attachment> attachments = new AttachmentsBL().GetByTypeAndId((int)En_LinkType.SupportTicket, id);
            return PartialView("_Attachments", attachments);
        }

        //Get Details of selected Support ticket.
        public ActionResult TicketDetail(Guid id)
        {
            Ticket oTicket = new SupportTicketBL().GetByTicketId(id);
            //lookups
            ViewBag.OperatorPriorities = new TicketPriorityBL().GetPrioritiesByType(Convert.ToInt16(En_Priority_Role.Operator));
            ViewBag.LstOperators = new UserBL().GetAllByRole(En_Role.Operator.ToString());
            ViewBag.LstSupportTeams = new SupportTeamBL().GetAll();
            ViewBag.TicketStatus = new TicketStatusBL().GetStatus();
            List<sp_GetSupportCategoryList_Result> lstCategories = new SupportCategoryBL().GetCompanyCategoryList(oTicket.CompanyId);
            ViewBag.lstcategory = from Category in lstCategories
                                  select new SelectListItem
                                  {
                                      Text = Category.CategoryName.ToString(),
                                      Value = Category.CategoryId.ToString()
                                  };

            if (new SupportTicketBL().IsSetCloseStatus(oTicket))
                ViewBag.CloseStatus = true;
            else
                ViewBag.CloseStatus = false;

            ViewBag.lstTicketsSource = from ticket in new TicketsSourcesBL().GetAllTicketSources()
                                       select new SelectListItem
                                       {
                                           Text = ticket.Description,
                                           Value = ticket.Id.ToString(),
                                       };
            ViewBag.DefaultForTicket = new TicketsSourcesBL().GetDefaultTicketSourceForTicket().Id;

            return PartialView("_TicketDetail", oTicket);
        }

        //Get All Events for Selected SupportTicket
        public ActionResult Events(Guid id)
        {
            return PartialView("_Events", id);
        }

        //Get SLA Contract Details for Selected SupportTicket
        public ActionResult SLAContracts(Guid id)
        {
            Ticket oTicket = new SupportTicketBL().GetByTicketId(id);
            vw_CompanyContract ovw_CompanyContract = new CompanyContractBL().GetCompanyContractDetail(oTicket.CompanyId, oTicket.ContractId);
            return PartialView("_SLAContracts", ovw_CompanyContract);
        }

        //Get Contact Details for Selected SupportTicket
        public ActionResult Contacts(Guid id)
        {
            Ticket oTicket = new SupportTicketBL().GetByTicketId(id);
            Company oCompany = new CompanyBL().GetById(oTicket.CompanyId);
            ViewBag.CompanyName = oCompany.CompanyName;
            ViewBag.CompanyContact = oCompany.Contact;

            User oUser = new UserBL().GetById(oTicket.CompanyUserId);
            ViewBag.UserContactDescription = oUser.ContactDescription;
            
            return PartialView("_Contacts", oUser);
        }

        //Get All Comments for Selected SupportTicket
        public ActionResult Comments(Guid id)
        {
            User oUser = SiteUtility.GetCurrentUser();
            Ticket oTicket = new SupportTicketBL().GetByTicketId(id);
            List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
            ViewBag.IsTicket_Open = lstOpenStatusId.Contains(oTicket.CurrentStatus) ? true : false;
            Comment oComment = new Comment();
            oComment.CreatedOpt_Id = oUser.UserId;
            oComment.TicketId = oTicket.TicketGuid;
            ViewBag.lstComments = new CommentBL().GetOperatorComment(id);
            return PartialView("_Comments", oComment);
        }

        //Get Phone calls grid data
        public ActionResult PhoneCalls(int id)
        {
            ViewBag.TicketId = id;
            return PartialView("_PhoneCalls");
        }
  
        //Get Activity Grid data
        public ActionResult Activity(int id)
        {
            ViewBag.TicketId = id;
            return PartialView("_Activity");
        }

        // Get Mails grid data
        public ActionResult Mails(int id)
        {
            ViewBag.TicketId = id;
            return PartialView("_Mails");
        }

        // Manage Activity
        public ActionResult ManageActivity(int TicketId,int id = 0)
        {
            TicketActivity oTicketActivity = new TicketActivity();
            oTicketActivity.TicketId = TicketId;

            if (id > 0)
                oTicketActivity = new TicketActivityBL().GetById(id);
         
            return PartialView("_ManageActivity",oTicketActivity);
        }

        #region DynamicDropDown

        //Get All Dynamic dropdown in Form
        public List<SelectListItem> GetCompanyUsers()
        {
            List<SelectListItem> CompanyUsers = new List<SelectListItem>();
            try
            {
                List<vw_CompanyUser> lstCompanyUsers = new UserBL().GetAllCompanyUser();
                foreach (vw_CompanyUser oCompanyUser in lstCompanyUsers)
                {
                    CompanyUsers.Add(new SelectListItem
                    {
                        Text = oCompanyUser.Name + " " + "(" + oCompanyUser.CompanyName + ")",
                        Value = oCompanyUser.UserId.ToString(),
                    });
                }
                return CompanyUsers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetContractByCompanyId(int selectedCompanyUser)
        {
            List<SelectListItem> Contracts = new List<SelectListItem>();
            int selectedCompany = new UserBL().GetById(selectedCompanyUser).CompanyId.Value;

            try
            {
                vw_CompanyContract oCompanyContract = new CompanyContractBL().GetActiveContractDetailByCompanyId(selectedCompany);
                if (oCompanyContract != null && oCompanyContract.TicketsLeft > 0)
                {
                    Contracts.Add(new SelectListItem
                    {
                        Text = oCompanyContract.TemplateName,
                        Value = oCompanyContract.CompanyContractId.ToString(),
                    });
                    return Json(new { success = true, lstContracts = Contracts, CompanyId = selectedCompany });
                }
                else
                    return Json(new { success = false, lstContracts = string.Empty });
            }
            catch (Exception)
            {
                return Json(new { success = false, lstContracts = Contracts });
            }
        }

        public JsonResult GetCategoryByCompanyId(int selectedCompany, int SelectedContract)
        {
            vw_CompanyContract ovw_CompanyContract = new CompanyContractBL().GetCompanyContractDetail(selectedCompany, SelectedContract);
            if (ovw_CompanyContract.TicketsLeft > 0)
            {
                List<SelectListItem> Categories = new List<SelectListItem>();
                try
                {
                    List<sp_GetSupportCategoryList_Result> lstCategories = new SupportCategoryBL().GetCompanyCategoryList(selectedCompany);
                    foreach (sp_GetSupportCategoryList_Result ocategory in lstCategories)
                    {
                        Categories.Add(new SelectListItem
                        {
                            Text = ocategory.CategoryName,
                            Value = ocategory.CategoryId.ToString(),
                        });
                    }
                    return Json(new { success = true, lstCategories = Categories });
                }
                catch (Exception)
                {
                    return Json(new { success = false, lstCategories = Categories });
                }
            }
            else
            {
                return Json(new { success = false, lstCategories = "NoTickets" });
            }

        }


        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadContractDetail(int CompanyId, int Contract)
        {
            vw_CompanyContract ovw_CompanyContract = new CompanyContractBL().GetCompanyContractDetail(CompanyId, Contract);

            return Json(ovw_CompanyContract, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region CRUD Methods

        #region Ticket

        //Create New or Update Existing SupportTicket and Mail Notification According to Action.
        public ActionResult Save(Ticket oTicket, FormCollection collection)
        {
            try
            {
                int id = oTicket.TicketId;
                User oUser = SiteUtility.GetCurrentUser();
                if (id == 0)
                {
                    oTicket.TicketGuid = Guid.NewGuid();
                    oTicket.CreatedByOpt = oUser.UserId;
                    Ticket _beforeCreateTicket = oTicket;
                    oTicket = new SupportTicketBL().Create(oTicket);
                    #region Email New Ticket Notification


                    vw_SupportTicket oSupportTicket = new SupportTicketBL().GetTicketViewDetailById(oTicket.TicketId);
                    User ocompUser = new UserBL().GetById(oTicket.CompanyUserId);
                    new EmailTemplateBL().Creation_ByOperator_ToContact(oSupportTicket, ocompUser.Email, "Admin");
                    if (oTicket.AssignToOperator != null)
                    {
                        bool _IsAutoAssigned = false;
                        if (_beforeCreateTicket.AssignToOperator == null)
                            _IsAutoAssigned = true;

                        User optUser = new UserBL().GetById(oTicket.AssignToOperator.Value);
                        new EmailTemplateBL().Creation_ByOperator_ToOperator(oSupportTicket, optUser.Email, optUser.Name,_IsAutoAssigned);
                    }
                    else if (oTicket.AssignToSupportTeam != null)
                    {
                        bool _IsAutoAssigned = false;
                        if (_beforeCreateTicket.AssignToSupportTeam == null)
                            _IsAutoAssigned = true;

                        List<int> UserIds = new SupportTeamMembersBL().GetByTeamId(oTicket.AssignToSupportTeam.Value).Select(p => p.UserId).ToList();
                        new EmailTemplateBL().Creation_ByOperator_ToTeam(oSupportTicket, UserIds, _IsAutoAssigned);
                    }
                    else if (oTicket.AssignToOperator == null && oTicket.AssignToSupportTeam == null)
                    {
                        Config oCongig = new ConfigsBL().GetByName(EntityNames.AdminEmail.ToString());
                        new EmailTemplateBL().Creation_ByOperator_ToOperator(oSupportTicket, oCongig.Value, "Admin",false);
                    }

                    #endregion

                    #region Attachments

                    List<Attachment> lstAttachment = new List<Attachment>();

                    if (collection["adImgs"] != null) //handle img for add only for edit it save directly.
                    {
                       

                        string[] adImgs = collection["adImgs"].Split(',');
                        string[] imgNames = collection["imgNames"].Split(',');

                        string tempDirectory = Server.MapPath(Constants.TempUploadPath);

                        for (int i = 0; i < adImgs.Count(); i++)
                        {
                            Image actualImg = null;
                            //read img from temp
                            string tempImgPath = Path.Combine(tempDirectory, imgNames[i]);
                            if (CommonFunction.IsImage(adImgs[i]))
                            {
                                actualImg = Image.FromFile(tempImgPath);
                                var attachment = SaveImageAndThumb(actualImg, imgNames[i], adImgs[i], oTicket.TicketId);
                                // lstAttachment.Add(attachment);
                                actualImg.Dispose();
                            }
                            else
                            {
                                string Directory = oTicket.TicketId.ToString();
                                string imgDirectory = Server.MapPath(Constants.TicketImgUploadPath + "/" + Directory);
                                if (!System.IO.Directory.Exists(imgDirectory))
                                    System.IO.Directory.CreateDirectory(imgDirectory);

                                System.IO.File.Move(Path.Combine(tempDirectory, imgNames[i]), Path.Combine(imgDirectory, imgNames[i]));

                                Attachment oAttachment = new Attachment();
                                oAttachment.FileName = imgNames[i];
                                oAttachment.FilePath = Directory + "/" + adImgs[i];
                                oAttachment.LinkType = (int)En_LinkType.SupportTicket;
                                lstAttachment.Add(oAttachment);
                            }

                            //delete file from temp
                            if (System.IO.File.Exists(tempImgPath))
                                System.IO.File.Delete(tempImgPath);
                        }
                        foreach (Attachment itm in lstAttachment)
                        {
                            itm.LinkId = oTicket.TicketId;
                            new AttachmentsBL().Create(itm);
                        }
                    }
                    #endregion
                }
                else
                {
                    vw_SupportTicket oldTicket = new SupportTicketBL().GetTicketViewDetailById(oTicket.TicketId);
                    oTicket= new SupportTicketBL().Update(oTicket, oUser.UserId);
                   
                    
                    #region Email Status Change Notification

                    List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
                    //This code block is for Notify any Status Change or Re-Open a Closed Ticket.
                    if (oldTicket.CurrentStatusId != oTicket.CurrentStatus && lstOpenStatusId.Contains(oTicket.CurrentStatus))
                    {
                        vw_SupportTicket oSupportTicket = new SupportTicketBL().GetTicketViewDetailById(oTicket.TicketId);
                        User ocompUser = new UserBL().GetById(oTicket.CompanyUserId);
                        new EmailTemplateBL().StatusChanged_ToContact(oSupportTicket, ocompUser.Email);
                        new EmailTemplateBL().StatusChanged_ToOperator(oSupportTicket, oUser.Email,"Admin",oldTicket.StatusName);
                    }
                    //This code block is for Notify status changed to Closed Ticket.
                    else if (!lstOpenStatusId.Contains(oTicket.CurrentStatus))
                    {
                        vw_SupportTicket oSupportTicket = new SupportTicketBL().GetTicketViewDetailById(oTicket.TicketId);
                        User ocompUser = new UserBL().GetById(oTicket.CompanyUserId);
                        new EmailTemplateBL().Closed_ToContact(oSupportTicket, ocompUser.Email);
                        new EmailTemplateBL().Closed_ToOperator(oSupportTicket, oUser.Email,"Admin",oldTicket.StatusName);
                    }

                    #endregion

                    #region Email ReAssign Notification

                    if (oTicket.AssignToOperator != null && oTicket.AssignToOperator != oldTicket.AssignToOperator)
                    {

                            vw_SupportTicket oSupportTicket = new SupportTicketBL().GetTicketViewDetailById(oTicket.TicketId);
                            User optUser = new UserBL().GetById(oTicket.AssignToOperator.Value);
                            new EmailTemplateBL().Reassignment(oSupportTicket, oUser.Name, oldTicket.AssignToName, optUser.Email, optUser.Name);
                    }
                    else if (oTicket.AssignToSupportTeam != null && oTicket.AssignToSupportTeam != oldTicket.AssignToSupportTeam)
                    {
                            List<int> UserIds = new SupportTeamMembersBL().GetByTeamId(oTicket.AssignToSupportTeam.Value).Select(p => p.UserId).ToList();
                            vw_SupportTicket oSupportTicket = new SupportTicketBL().GetTicketViewDetailById(oTicket.TicketId);
                            new EmailTemplateBL().Reassignment_ToTeam(oSupportTicket, oUser.Name, oldTicket.AssignToName, UserIds);
                    }
                    #endregion
                }

                TempData["successmsg"] = CommonMsg.Success(EntityNames.SupportTicket, id == 0 ? En_CRUD.Insert : En_CRUD.Update);
                return RedirectToAction("View_Detail", new { id = oTicket.TicketGuid, tab = "TicketView" });
            }
            catch (Exception ex)
            {

                TempData["errormsg"] = CommonMsg.Error();
                return RedirectToAction("Index");
            }
        }

        //Delete Selected SupportTicket
        public JsonResult Delete(int id)
        {
            try
            {
                new SupportTicketBL().Delete(id);
                return Json(new { success = true, Message = CommonMsg.Success_Delete(EntityNames.SupportTicket) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = CommonMsg.Fail_Delete(EntityNames.SupportTicket) });
            }
        }

        #endregion

        #region Comment

        //Create new Comment and mail Notification
        public JsonResult SaveComment(Comment oComment)
        {
            try
            {
                User oUser = SiteUtility.GetCurrentUser();
                oComment= new CommentBL().Create(oComment);
                
                    #region Email Comment Notification
                    vw_SupportTicket oSupportTicket = new SupportTicketBL().GetTicketViewDetailByGuid(oComment.TicketId);
                    User optUser = new UserBL().GetById(oSupportTicket.CompanyUserId);
                    new EmailTemplateBL().Creation_ByOperator(oSupportTicket, oComment, optUser.Email,oUser.Name,oComment.IsPrivate);
                    #endregion

                return Json(new { success = true, TicketId = oComment.TicketId, message = CommonMsg.Success(EntityNames.Comment, En_CRUD.Update) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        #endregion

        #region Activity

        //Save Activity
        public JsonResult SaveActivity(TicketActivity oTicketActivity)
        {
            try
            {
                bool Add_Flg = new CommonBL().isNewEntry(oTicketActivity.Id);

                if (Add_Flg)
                    new TicketActivityBL().Create(oTicketActivity);
                else
                    new TicketActivityBL().Update(oTicketActivity);

                Ticket oTicket = new SupportTicketBL().GetById(oTicketActivity.TicketId);

                return Json(new { success = true, message = CommonMsg.Success(EntityNames.TicketActivity, Add_Flg == true ? En_CRUD.Insert : En_CRUD.Update), id = oTicket.TicketGuid });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        //Delete Activity by Id
        public JsonResult DeleteActivity(int id)
        {
            try
            {
                new TicketActivityBL().Delete(id);
                return Json(new { success = true, Message = CommonMsg.Success_Delete(EntityNames.TicketActivity) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = CommonMsg.Fail_Delete(EntityNames.TicketActivity) });
            }
        }

        #endregion

        #region Mails



        #endregion

        #endregion

        #region Helper
        //To Check If Permission granted for Selected Support Ticket
        public bool IsOwnTicket(Ticket oTicket)
        {
            User oUser = SiteUtility.GetCurrentUser();
            List<int> lstTeamId = (List<int>)Session[En_UserSession.SupportTeams.ToString()];

            if (oTicket.AssignToOperator != null && oTicket.AssignToOperator == oUser.UserId)
                return true;
            else if (oTicket.AssignToSupportTeam != null && lstTeamId != null && lstTeamId.Contains(oTicket.AssignToSupportTeam.Value))
                return true;

            return false;

        }
        public bool IsEditOtherAllow(Ticket oTicket)
        {
            if (IsOwnTicket(oTicket))
                return true;
            else
            {
                if (SiteUtility.IsAllow((int)En_Permission.EditOthersTickets))
                    return true;
            }
            return false;
        }

        public bool IsReOpenAllow(Ticket oTicket)
        {
            if (IsOwnTicket(oTicket))
            {
                if (SiteUtility.IsAllow((int)En_Permission.ReOpenClosedTickets))
                    return true;
            }
            else
            {
                if (SiteUtility.IsAllow((int)En_Permission.EditOthersTickets))
                {
                    if (SiteUtility.IsAllow((int)En_Permission.ReOpenClosedTickets))
                        return true;
                }
            }

            return false;
        }

        public string GetStatus()
        {

            try
            {
                List<TicketStatu> lstTicketStatu = new TicketStatusBL().GetAllOpenStatus();

                string Status = ":(Show All Open);";
                foreach (TicketStatu oTicketStatu in lstTicketStatu)
                {
                    Status = Status + oTicketStatu.StatusName + ":" + oTicketStatu.StatusName + ";";
                }
                Status = Status.Remove(Status.Length - 1);
                return Status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetOperatorPriority()
        {

            try
            {
                List<TicketPriority> lstOptPriority = new TicketPriorityBL().GetPrioritiesByType(Convert.ToInt16(En_Priority_Role.Operator));
                string Status = ":(Show All...);";
                foreach (TicketPriority oTicketPriority in lstOptPriority)
                {
                    Status = Status + oTicketPriority.Description + ":" + oTicketPriority.Description + ";";
                }
                Status = Status.Remove(Status.Length - 1);
                return Status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetCustomerPriority()
        {

            try
            {
                List<TicketPriority> lstCustomerPriority = new TicketPriorityBL().GetPrioritiesByType(Convert.ToInt16(En_Priority_Role.Customer));

                string Status = ":(Show All...);";
                foreach (TicketPriority oTicketPriority in lstCustomerPriority)
                {
                    Status = Status + oTicketPriority.Description + ":" + oTicketPriority.Description + ";";
                }
                Status = Status.Remove(Status.Length - 1);
                return Status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetCategories()
        {

            try
            {
                List<sp_GetSupportCategoryList_Result> lstCategories = new SupportCategoryBL().GetCategoryDDL(2);

                string Status = ":(Show All...);";
                foreach (sp_GetSupportCategoryList_Result oTicketPriority in lstCategories)
                {
                    Status = Status + oTicketPriority.CategoryName + ":" + oTicketPriority.CategoryName + ";";
                }
                Status = Status.Remove(Status.Length - 1);
                return Status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetCompanyAllUsers()
        {

            try
            {
                List<Company> lstCompany = new CompanyBL().GetAllEnableCompanies();
                string Status = ":(Show All...);";
                foreach (Company oCompany in lstCompany)
                {
                    List<User> lstCompanyUsers = new UserBL().GetUsersByCompanyId(oCompany.CompanyId);
                    foreach (User oCompanyUser in lstCompanyUsers)
                    {
                        Status = Status + oCompanyUser.Name + ":" + oCompanyUser.Name + ";";
                    }
                }
                Status = Status.Remove(Status.Length - 1);
                return Status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Attachment

        //Insert Attachment files
        public ActionResult AddFiles(int id)
        {
            string uploadDirectory = Server.MapPath(Constants.TicketImgUploadPath + "/" + id);
            if (!System.IO.Directory.Exists(uploadDirectory))
                System.IO.Directory.CreateDirectory(uploadDirectory);

            string fileId = "";
            string FileNames = "";

            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                if (file != null && file.ContentLength > 0)
                {
                    fileId = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    if (CommonFunction.IsImage(file.FileName))
                    {
                        Image actualImg = Image.FromStream(file.InputStream);
                        actualImg.Save(string.Format("{0}\\{1}", uploadDirectory, fileId));

                        //create thumb and save
                        var thumbImg = CommonFunction.ResizeImage(actualImg, 160, 160);
                        string thumbImgName = Path.GetFileNameWithoutExtension(fileId) + "_thumb" + Path.GetExtension(fileId);
                        thumbImg.Save(string.Format("{0}\\{1}", uploadDirectory, thumbImgName));
                    }
                    else
                    {   //save file
                        file.SaveAs(string.Format("{0}\\{1}", uploadDirectory, fileId));
                    }

                    //save to db
                    Attachment oAttachment = new Attachment();
                    oAttachment.LinkId = int.Parse(id.ToString());
                    oAttachment.FileName = file.FileName;
                    oAttachment.FilePath = string.Format("{0}\\{1}", id, fileId);
                    oAttachment.LinkType = (int)En_LinkType.SupportTicket;

                    new AttachmentsBL().Create(oAttachment);
                }
                FileNames = FileNames + file.FileName + ",";
            }
            if (FileNames != "")
            {
                #region Send Attachment mail

                vw_SupportTicket VwTicket = new SupportTicketBL().GetTicketViewDetailById(Convert.ToInt32(id));
                string link = Utility.CommonFunction.GetSite_URL() + "/Admin/SupportTicket/View_Detail?id=" + VwTicket.TicketGuid + "&tab=Attachments";

                new AttachmentsBL().SendAttachmentMail(En_EmailKey.Creation_ByContact.ToString(), VwTicket,link,FileNames);

                #endregion
            }

            return Json(new { Message = string.Empty });
        }

        // delete attchments
        public JsonResult DeleteFile(int id)
        {
            try
            {
                AttachmentsBL attachmentBL = new AttachmentsBL();
                attachmentBL.Delete(id, Constants.TicketImgUploadPath.ToString());
                return Json(new { success = true, message = CommonMsg.Success(EntityNames.Attachment, En_CRUD.Delete) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Fail(EntityNames.Attachment, En_CRUD.Update) });
            }
        }

        // download file
        public FileContentResult DownloadFile(int id)
        {
            Attachment attachment = new AttachmentsBL().GetById(id);
            string contentType = CommonFunction.GetMimeType(Path.GetExtension(attachment.FileName));
            return File(System.IO.File.ReadAllBytes(Server.MapPath(Constants.TicketImgUploadPath + "/" + attachment.FilePath)), contentType, attachment.FileName);
        }

        #endregion

        #region Image Upload/Delete
        private Attachment SaveImageAndThumb(Image actualImg, string OriginalImg,string imgName, int TicketId)
        {
            string Directory = TicketId.ToString();
            string imgDirectory = Server.MapPath(Constants.TicketImgUploadPath + "/" + Directory);
            if (!System.IO.Directory.Exists(imgDirectory))
                System.IO.Directory.CreateDirectory(imgDirectory);

            //resize to max height/width
            actualImg = SiteUtility.ResizeImage(actualImg, 700, 700);

            //save to proper folder
            actualImg.Save(string.Format("{0}\\{1}", imgDirectory, imgName));

            //create thumb and save
            var thumbImg = SiteUtility.ResizeImage(actualImg, 160, 160);
            string thumbImgName = Path.GetFileNameWithoutExtension(imgName) + "_thumb" + Path.GetExtension(imgName);
            thumbImg.Save(string.Format("{0}\\{1}", imgDirectory, thumbImgName));

            //save to db

            Attachment oAttachment = new Attachment();
            oAttachment.LinkId = TicketId;
            oAttachment.FileName = OriginalImg;
            oAttachment.FilePath = Directory + "/" + imgName;
            oAttachment.LinkType = (int)En_LinkType.SupportTicket;

            if (TicketId > 0)
                new AttachmentsBL().Create(oAttachment);

            return oAttachment;
        }

        public ActionResult SaveUploadedFile(int TicketId)
        {
            if (TicketId > 0)
            {
                string fileId = "";
                int imgId = 0;
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    if (file != null && file.ContentLength > 0)
                    {
                        fileId = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        if (CommonFunction.IsImage(file.FileName))
                        {
                            var oImage = SaveImageAndThumb(Image.FromStream(file.InputStream), file.FileName,fileId, TicketId);
                            imgId = oImage.AttachmentId;
                        }
                        else
                        {
                            string Directory = TicketId.ToString();
                            string imgDirectory = Server.MapPath(Constants.ImgUploadPath + "/" + Directory);
                            if (!System.IO.Directory.Exists(imgDirectory))
                                System.IO.Directory.CreateDirectory(imgDirectory);

                            if (Request.Files[fileName].ContentLength == 0) continue;
                            string filename = Path.GetFileName(Request.Files[fileName].FileName);
                            Request.Files[fileName].SaveAs(Path.Combine(imgDirectory, filename));

                            Attachment oAttachment = new Attachment();
                            oAttachment.LinkId = TicketId;
                            oAttachment.FileName = filename;
                            oAttachment.FilePath = Directory + "/" + filename;
                            oAttachment.LinkType = (int)En_LinkType.SupportTicket;
                            new AttachmentsBL().Create(oAttachment);
                        }
                    }
                }
                return Json(new { success = "true", fileId = imgId, message = "Image uploaded successfully." });
            }
            else
            {
                string serverId = "";
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    if (file != null && file.ContentLength > 0)
                    {
                        serverId = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        string tempDirectory = Server.MapPath(Constants.TempUploadPath);

                        if (!System.IO.Directory.Exists(tempDirectory))
                            System.IO.Directory.CreateDirectory(tempDirectory);

                        var path = string.Format("{0}\\{1}", tempDirectory, file.FileName);
                        file.SaveAs(path);
                    }
                }
                return Json(new { success = "true", serverId = serverId });
            }
        }

        public ActionResult DeleteUploadedFile(string serverId, int fileId)
        {
            if (fileId > 0) //delete from db
            {
                Attachment oAttachment = new AttachmentsBL().GetById(fileId);

                //delete img and thumb from folder
                string imgDirectory = Server.MapPath(Constants.TicketImgUploadPath);
                string filePath = Path.Combine(imgDirectory, oAttachment.FilePath);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                filePath = Path.Combine(imgDirectory, oAttachment.FilePath.Remove(oAttachment.FilePath.LastIndexOf('.')) + "_thumb"
                                            + Path.GetExtension(oAttachment.FilePath));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                //delete img from db
                new AttachmentsBL().RemoveImage(fileId);

                return Json(new { success = "true", message = "Image deleted successfully." });
            }
            else
            {
                string tempDirectory = Server.MapPath(Constants.TempUploadPath);
                string filePath = Path.Combine(tempDirectory, serverId);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                return Json(new { success = "true" });
            }
        }

        #endregion
    }
}