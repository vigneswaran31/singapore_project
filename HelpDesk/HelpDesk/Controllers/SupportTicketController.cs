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
using System.Text;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace HelpDesk.Controllers
{
    public class SupportTicketController : BaseController
    {
        #region Grid Methods

        private void LanguageHeader()
        {
            // Language Header
            ViewBag.LngWelcome = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Welcome);
            ViewBag.LngChangePassword = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ChangePassword);
            ViewBag.LngMyProfile = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.MyProfile);
            ViewBag.LngLogout = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Logout);
        }

        //Load Support Ticket Main Page
        public ActionResult Index()
        {
            ViewBag.ddlStatus = GetStatus();
            ViewBag.ddlCusPriorities = GetCustomerPriority();
            ViewBag.ddlCategories = GetCategories();

            //Language
            ViewBag.CreateSupportTicket = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.CreateSupportTicket);
            ViewBag.AdvancedSearch = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.AdvancedSearch);
            ViewBag.Title = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Title);
            ViewBag.Category = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Category);
            ViewBag.Company = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Company);
            ViewBag.CompanyUser = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.CompanyUser);
            ViewBag.SolPri = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.SolPri);
            ViewBag.AssignTo = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.AssignTo);
            ViewBag.ProblemDescription = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ProblemDescription);

            LanguageHeader();
            return View();
        }

        //Load Support tickets in Grid Format for Display in Grid.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                User oUser = SiteUtility.GetCurrentUser();
                return new SupportTicketBL().GetAllUserTickets(grid, oUser.UserId);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Advance search order grid data
        public string GetAdvSearchResult(GridSettings grid, UserSearchTicketModel AdvSearch)
        {
            try
            {
                User oUser = SiteUtility.GetCurrentUser();
                return new SupportTicketBL().GetAllUserTickets(grid, oUser.UserId, AdvSearch);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string GetPhoneCallsGridData(GridSettings grid, int Id)
        {
            try
            {
                return new PhoneCallsBL().GetGridDataByTicketId(grid, Id);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

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

                //Language
                ViewBag.LngYouAreLookingAtTicket = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.YouAreLookingAtTicket);
                ViewBag.LngCreatedOn = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.CreatedOn);
                ViewBag.LngClosedOn = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ClosedOn);
                ViewBag.LngNotClosedYet = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.NotClosedYet);
                ViewBag.LngNextActionType = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.NextActionType);
                ViewBag.LngNextActionDueFor = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.NextActionDueFor);
                ViewBag.LngTicketDetails = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.TicketDetails);
                ViewBag.LngComments = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Comments);
                ViewBag.LngAttachments = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Attachments);

                LanguageHeader();

                if (new ContractTemplateBL().IsSlaAllowOnContractById(ovw_SupportTicket.ContractId))
                {
                    if (ovw_SupportTicket.ResponseStatus == "ResDone" && ovw_SupportTicket.SolutionStatus == "SolDone")
                    {
                        ViewBag.NextActionType = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.None);
                        ViewBag.NextActionDueFor = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.None); 
                    }
                    else if (ovw_SupportTicket.ResponseStatus == "ResPending")
                    {
                        ViewBag.NextActionType = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Response);
                        ViewBag.NextActionDueFor = ovw_SupportTicket.ResponseDeadline.ToString();
                    }
                    else if (ovw_SupportTicket.SolutionStatus == "SolPending")
                    {
                        ViewBag.NextActionType = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Solution);
                        ViewBag.NextActionDueFor = ovw_SupportTicket.SolutionDeadline.ToString();
                    }
                    else if (ovw_SupportTicket.ResponseStatus == "ResOutofSLA" && ovw_SupportTicket.SolutionStatus == "SolOutofSLA")
                    {
                        ViewBag.NextActionType = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.OutOfSLA);
                        ViewBag.NextActionDueFor = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.OutOfSLA); 
                    }
                }
            }
            return View(id);
        }

        //Load Advance search panel.
        public ActionResult AdvancedSearch()
        {
            User oUser = SiteUtility.GetCurrentUser();
            ViewBag.ddlStatus = new TicketStatusBL().GetStatus();
            ViewBag.ddlCompanyUsers = new UserBL().GetAllCompanyUserByCompanyId(oUser.CompanyId.Value);
            ViewBag.ddlSolutionPriorities = new TicketPriorityBL().GetPrioritiesByType(Convert.ToInt16(En_Priority_Role.Customer));
            ViewBag.ddlAssgnedToOperators = new UserBL().GetAllByRole(En_Role.Operator.ToString());
            ViewBag.ddlAssgnedToSupportTeams = new SupportTeamBL().GetAll();

            List<sp_GetSupportCategoryList_Result> lstCategories = new SupportCategoryBL().GetCategoryDDL(2);
            ViewBag.ddlCategories = from Category in lstCategories
                                    select new SelectListItem
                                    {
                                        Text = Category.CategoryName.ToString(),
                                        Value = Category.CategoryId.ToString()
                                    };

            LanguageHeader();

            return PartialView("_AdvancedSearch");
        }

        //Load New Support Ticket Form.
        public ActionResult Create()
        {
            User oUser = SiteUtility.GetCurrentUser();
            vw_CompanyContract oCompanyContract = new CompanyContractBL().GetActiveContractDetailByCompanyId(oUser.CompanyId.Value);
            if (oCompanyContract != null)
            {
                if (oCompanyContract.TicketsLeft > 0)
                {
                    Ticket oTicket = new Ticket();

                    //lookups
                    ViewBag.CustomerPriorities = new TicketPriorityBL().GetPrioritiesByType(Convert.ToInt16(En_Priority_Role.Customer));
                    ViewBag.Categories = new SupportCategoryBL().GetCompanyCategoryList(oUser.CompanyId.Value);

                    oTicket.ContractId = new CompanyContractBL().GetActiveContractDetailByCompanyId(oUser.CompanyId.Value).CompanyContractId;
                    oTicket.CurrentStatus = new TicketStatusBL().GetDefaultStatusForNewTicket().TicketStatusId;
                    oTicket.OperatorPriority = new TicketPriorityBL().GetDefaultPriorityByType(Convert.ToInt16(En_Priority_Role.Operator)).TicketPriorityId;
                    oTicket.CustomerPriority = new TicketPriorityBL().GetDefaultPriorityByType(Convert.ToInt16(En_Priority_Role.Customer)).TicketPriorityId;
                    oTicket.CompanyId = oUser.CompanyId.Value;
                    oTicket.CompanyUserId = oUser.UserId;

                    List<Attachment> attachments = new AttachmentsBL().GetByTypeAndId((int)En_LinkType.SupportTicket, oTicket.TicketId);
                    ViewBag.Attachments = JsonConvert.SerializeObject(attachments);

                    //Language
                    ViewBag.LngProblemDescription = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ProblemDescription);
                    ViewBag.LngSolutionPriority = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.SolutionPriority);
                    ViewBag.LngIssueCategory = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.IssueCategory);
                    ViewBag.LngAttachments = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Attachments);
                    ViewBag.LngSave = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Save);
                    ViewBag.LngCancel = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Cancel);
                    ViewBag.LngTitle = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Title);

                    LanguageHeader();

                    return View(oTicket);
                }
                else
                {
                    TempData["errormsg"] = "you can not create a new ticket  All Tickets are used for existing Contract.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
               TempData["errormsg"] = "you can not create a new ticket, There is no Active Contract exist for your company.";
               return RedirectToAction("Index");
            }
        }

        //Get Details of selected Support ticket.
        public ActionResult TicketDetail(Guid id)
        {
            Ticket oTicket = new SupportTicketBL().GetByTicketId(id);
            
            //lookups
            ViewBag.CustomerPriorities = new TicketPriorityBL().GetPrioritiesByType(Convert.ToInt16(En_Priority_Role.Customer));
            ViewBag.lstAttachment = new AttachmentsBL().GetByTypeAndId((int)En_LinkType.SupportTicket, oTicket.TicketId);

            //Language
            ViewBag.LngTitle = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Title);
            ViewBag.LngProblemDescription = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ProblemDescription);
            ViewBag.LngContactInfo = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ContactInfo);
            ViewBag.LngCategory = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Category);
            ViewBag.LngSave = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Save);
            ViewBag.LngCancel = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Cancel);
            ViewBag.LngCustomerPriority = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.CustomerPriority);

            LanguageHeader();

            return PartialView("_TicketDetail", oTicket);
        }

        //Get All Comments for Selected SupportTicket
        public ActionResult Comments(Guid id)
        {
            User oUser = SiteUtility.GetCurrentUser();
            Ticket oTicket = new SupportTicketBL().GetByTicketId(id);
            List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
            ViewBag.IsTicket_Open = lstOpenStatusId.Contains(oTicket.CurrentStatus) ? true : false;
            Comment oComment = new Comment();
            oComment.CreatedUser_Id = oUser.UserId;
            oComment.TicketId = oTicket.TicketGuid;
            ViewBag.lstComments = new CommentBL().GetCustomerComment(oTicket.TicketGuid);

            //Language
            ViewBag.LngCommentsForThisTicket = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.CommentsForThisTicket);
            ViewBag.LngOperator = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Operator);
            ViewBag.LngCustomer = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Customer);
            ViewBag.LngPrivateComment = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.PrivateComment);
            ViewBag.LngThereAreNoComments_ = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ThereAreNoComments_);
            ViewBag.LngAddComment = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.AddComment);

            LanguageHeader();

            return PartialView("_Comments", oComment);
        }

        //Get All Attachments for Selected SupportTicket
        public ActionResult Attachments(int id)
        {
            Ticket oTicket = new SupportTicketBL().GetById(id);
            List<Attachment> attachments = new AttachmentsBL().GetByTypeAndId((int)En_LinkType.SupportTicket, id);
            ViewBag.TicketId = id;
            List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
            ViewBag.IsTicket_Open = lstOpenStatusId.Contains(oTicket.CurrentStatus) ? true : false;

             //Language
            ViewBag.LngAreYouSure_DeleteThisAttachment = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.AreYouSure_DeleteThisAttachment);
            ViewBag.LngAddAttachment = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.AddAttachment);
            ViewBag.LngNoFilesAttached = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.NoFilesAttached);
            ViewBag.LngDownload = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Download);
            ViewBag.LngDelete = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Delete);

            LanguageHeader();

            return PartialView("_Attachments", attachments);
        }

        // Get Phone calls grid data
        public ActionResult PhoneCalls(int id)
        {
            ViewBag.TicketId = id;
            return PartialView("_PhoneCalls");
        }

        //Get Mails grid data
        public ActionResult Mails(int id)
        {
            ViewBag.TicketId = id;
            return PartialView("_Mails");
        }

        //Get Activity grid data
        public ActionResult Activity(int id)
        {
            ViewBag.TicketId = id;
            return PartialView("_Activity");
        }


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
                    oTicket.CreatedByUser = oUser.UserId;

                    oTicket = new SupportTicketBL().Create(oTicket);
                    #region Email New Ticket Notification

                    vw_SupportTicket vwSupportTicket = new SupportTicketBL().GetTicketViewDetailById(oTicket.TicketId);

                    new EmailTemplateBL().Creation_ByContact_ToContact(vwSupportTicket, oUser.Email, oUser.Name,oUser.LanguageId.Value);
                    if (oTicket.AssignToOperator != null)
                    {
                        User optUser = new UserBL().GetById(oTicket.AssignToOperator.Value);
                        new EmailTemplateBL().Creation_ByContact_ToOperator(vwSupportTicket, optUser.Email, optUser.Name, oUser.LanguageId.Value);
                    }
                    else if (oTicket.AssignToSupportTeam != null)
                    {
                        List<int> UserIds = new SupportTeamMembersBL().GetByTeamId(oTicket.AssignToSupportTeam.Value).Select(p => p.UserId).ToList();
                        new EmailTemplateBL().Creation_ByContact_ToTeam(vwSupportTicket, UserIds, oUser.LanguageId.Value);
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
                    new SupportTicketBL().Update(oTicket, oUser.UserId);

                TempData["successmsg"] = CommonMsg.Success(EntityNames.SupportTicket, id == 0 ? En_CRUD.Insert : En_CRUD.Update);
                return RedirectToAction("View_Detail", new { id = oTicket.TicketGuid, tab = "TicketView" });
            }
            catch (Exception ex)
            {

                TempData["errormsg"] = CommonMsg.Error();
                return RedirectToAction("Index");
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
                    if (oSupportTicket.AssignToOperator != null)
                    {
                        User optUser = new UserBL().GetById(oSupportTicket.AssignToOperator.Value);
                        new EmailTemplateBL().CommmentCreation_ByContact_ToOperator(oSupportTicket, oComment, optUser.Email, oUser.Name, oUser.LanguageId.Value);
                    }
                    else if (oSupportTicket.AssignToSupportTeam != null)
                    {
                        List<int> UserIds = new SupportTeamMembersBL().GetByTeamId(oSupportTicket.AssignToSupportTeam.Value).Select(p => p.UserId).ToList();
                        new EmailTemplateBL().CommmentCreation_ByContact_ToTeam(oSupportTicket, oComment, UserIds, oUser.Name, oUser.LanguageId.Value);
                    }
                    else if (oSupportTicket.AssignToOperator == null && oSupportTicket.AssignToSupportTeam == null)
                    {
                        Config oCongig = new ConfigsBL().GetByName(EntityNames.AdminEmail.ToString());
                        new EmailTemplateBL().CommmentCreation_ByContact_ToOperator(oSupportTicket, oComment, oCongig.Value, "Admin", oUser.LanguageId.Value);
                    }

                    #endregion


                    return Json(new { success = true,TicketId=oComment.TicketId, message = CommonMsg.Success(EntityNames.Comment, En_CRUD.Update) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        #endregion
        #endregion

        #region Helper
        //To Check If Permission granted for Selected Support Ticket
        public JsonResult IsActiveContract()
        {
            User oUser = SiteUtility.GetCurrentUser();
            try
            {
                vw_CompanyContract oCompanyContract = new CompanyContractBL().GetActiveContractDetailByCompanyId(oUser.CompanyId.Value);
                if (oCompanyContract != null)
                {
                    if (oCompanyContract.TicketsLeft > 0)
                        return Json(new { success = true });
                    else
                        return Json(new { success = false, errormsg = "NoTicket" });
                }
                else
                {
                    return Json(new { success = false, errormsg = "NoActiveContract" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
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
                string link = Utility.CommonFunction.GetSite_URL() + "/SupportTicket/View_Detail?id=" + VwTicket.TicketGuid + "&tab=Attachments";

                new AttachmentsBL().SendAttachmentMail(En_EmailKey.Creation_ByContact.ToString(), VwTicket, link, FileNames, SiteUtility.GetCurrentUser().LanguageId.Value);

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
        private Attachment SaveImageAndThumb(Image actualImg, string OriginalImg, string imgName, int TicketId)
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
                            var oImage = SaveImageAndThumb(Image.FromStream(file.InputStream), file.FileName, fileId, TicketId);
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