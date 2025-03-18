using HelpDeskEntity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HelpDeskBAL
{
    public class EmailTemplateBL
    {
        #region Get Methods

        //Get EmailTemplate record by Key
        public EmailTemplate GetByKeyLanguage(string Key,int LanguageId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.EmailTemplates.Where(p => p.Key == Key && p.LanguageId == LanguageId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Email Template data list in Grid Format.
        public string GetGridData(GridSettings grid,int LangId = 0)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.EmailTemplates.AsQueryable();

                    if(LangId > 0)
                        query = ctx.EmailTemplates.Where(p => p.LanguageId == LangId).AsQueryable();

                    int count;
                    var data = query.GridCommonSettings(grid, out count);
                    var result = new
                    {
                        total = (int)Math.Ceiling((double)count / grid.PageSize),
                        page = grid.PageIndex,
                        records = count,
                        rows = (from p in data
                                select new
                                {
                                    EmailTemplateId = p.EmailTemplateId,
                                    EmailType = p.EmailType,
                                    Subject = p.Subject,
                                    IsHtml = p.IsHtml,
                                    IsEnable = p.IsEnable,
                                    Action = p.EmailTemplateId
                                }).ToArray()
                    };
                    return JsonConvert.SerializeObject(result, new IsoDateTimeConverter());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //get emailtemplate object by Id
        public EmailTemplate GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.EmailTemplates.Where(p => p.EmailTemplateId == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion

        #region Email Send Operation

        public void Creation_ByContact_ToContact(vw_SupportTicket oTicket, string Email, string Name,int LanguageId = 1)
        {

            string link = Utility.CommonFunction.GetSite_URL() + "/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=TicketView";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("Creation_ByContact_ToContact",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[TICKET_DESCRIPTION]]", oTicket.ProblemDescription);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);

                oEmailQueue.To = Email;
                oEmailQueue.ToName = oTicket.CompanyUserName;
                oEmailQueue.Subject = oEmailTemplate.Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                EmailQueueBL.SendMail(oEmailQueue, true);
            }
            Event oEvent = new Event();
            oEvent.TicketId = oTicket.TicketGuid;
            oEvent.EventDescription = Name + "(User) has Open Support Ticket";
            new EventBL().Create(oEvent);
        }

        public void Creation_ByContact_ToOperator(vw_SupportTicket oTicket, string Email, string Name, int LanguageId = 1)
        {

            string link = Utility.CommonFunction.GetSite_URL() + "/Account/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=TicketView";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("Creation_ByContact_ToOperator",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[TICKET_DESCRIPTION]]", oTicket.ProblemDescription);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);
                string Subject = oEmailTemplate.Subject;
                Subject = Subject.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);

                oEmailQueue.To = Email;
                oEmailQueue.ToName = oTicket.CompanyUserName;
                oEmailQueue.Subject = Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                EmailQueueBL.SendMail(oEmailQueue, true);
            }
            Event oEvent = new Event();
            oEvent.TicketId = oTicket.TicketGuid;
            oEvent.EventDescription = Name + "(Operator) is Assigned to Ticket";
            new EventBL().Create(oEvent);
        }

        public void Creation_ByContact_ToTeam(vw_SupportTicket oTicket, List<int> UserIds, int LanguageId = 1)
        {
            string link = Utility.CommonFunction.GetSite_URL() + "/Account/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=TicketView";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("Creation_ByContact_ToOperator",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[TICKET_DESCRIPTION]]", oTicket.ProblemDescription);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);
                string Subject = oEmailTemplate.Subject;
                Subject = Subject.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);

                oEmailQueue.Subject = Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                foreach (int item in UserIds)
                {
                    User optUser = new UserBL().GetById(item);
                    if (oEmailQueue.To == null)
                    {
                        oEmailQueue.To = optUser.Email;
                        oEmailQueue.ToName = optUser.Name;
                    }
                    else
                        oEmailQueue.CC = oEmailQueue.CC + optUser.Email + ",";
                }

                oEmailQueue.CC = oEmailQueue.CC.TrimEnd(',');
                EmailQueueBL.SendMail(oEmailQueue, true);

            }
            Event oEvent = new Event();
            oEvent.TicketId = oTicket.TicketGuid;
            oEvent.EventDescription = oTicket.AssignToName + "(Team) has Assigned New Support Ticket";
            new EventBL().Create(oEvent);
        }

        public void Creation_ByOperator_ToContact(vw_SupportTicket oTicket, string Email, string Name, int LanguageId = 1)
        {

            string link = Utility.CommonFunction.GetSite_URL() + "/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=TicketView";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("Creation_ByOperator_ToContact",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[TICKET_DESCRIPTION]]", oTicket.ProblemDescription);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);

                oEmailQueue.To = Email;
                oEmailQueue.ToName = oTicket.CompanyUserName;
                oEmailQueue.Subject = oEmailTemplate.Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                EmailQueueBL.SendMail(oEmailQueue, true);
            }
            Event oEvent = new Event();
            oEvent.TicketId = oTicket.TicketGuid;
            oEvent.EventDescription = Name + "(Operator) has Open Support Ticket";
            new EventBL().Create(oEvent);
        }

        public void Creation_ByOperator_ToOperator(vw_SupportTicket oTicket, string Email, string Name, bool IsAutoAssigned, int LanguageId = 1)
        {
            string link = Utility.CommonFunction.GetSite_URL() + "/Account/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=TicketView";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("Creation_ByOperator_ToOperator",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[TICKET_DESCRIPTION]]", oTicket.ProblemDescription);
                body = body.Replace("[[TICKET_PRIORITY_OPERATOR]]", oTicket.OperatorPriority);
                body = body.Replace("[[OPERATOR_CREATING_DISPLAYNAME]]", oTicket.CreatedByOperatorName);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);
                string Subject = oEmailTemplate.Subject;
                Subject = Subject.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);

                oEmailQueue.To = Email;
                oEmailQueue.ToName = oTicket.CompanyUserName;
                oEmailQueue.Subject = Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                EmailQueueBL.SendMail(oEmailQueue, true);
            }
            Event oEvent = new Event();
            oEvent.TicketId = oTicket.TicketGuid;
            if (IsAutoAssigned)
                oEvent.EventDescription = Name + "(Operator) is Auto Assigned to the Ticket";
            else
                oEvent.EventDescription = Name + "(Operator) is Assigned to the Ticket";

            new EventBL().Create(oEvent);
        }

        public void Creation_ByOperator_ToTeam(vw_SupportTicket oTicket, List<int> UserIds, bool IsAutoAssigned, int LanguageId = 1)
        {
            string link = Utility.CommonFunction.GetSite_URL() + "/Account/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=TicketView";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("Creation_ByOperator_ToOperator",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[TICKET_DESCRIPTION]]", oTicket.ProblemDescription);
                body = body.Replace("[[TICKET_PRIORITY_OPERATOR]]", oTicket.OperatorPriority);
                body = body.Replace("[[OPERATOR_CREATING_DISPLAYNAME]]", oTicket.CreatedByOperatorName);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);
                string Subject = oEmailTemplate.Subject;
                Subject = Subject.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);

                oEmailQueue.Subject = Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                foreach (int item in UserIds)
                {
                    User optUser = new UserBL().GetById(item);
                    if (oEmailQueue.To == null)
                    {
                        oEmailQueue.To = optUser.Email;
                        oEmailQueue.ToName = optUser.Name;
                    }
                    else
                        oEmailQueue.CC = oEmailQueue.CC + optUser.Email + ",";
                }

                oEmailQueue.CC = oEmailQueue.CC.TrimEnd(',');
                EmailQueueBL.SendMail(oEmailQueue, true);
            }
            Event oEvent = new Event();
            oEvent.TicketId = oTicket.TicketGuid;
            if (IsAutoAssigned)
                oEvent.EventDescription = oTicket.AssignToName + "(Team) is Auto Assigned to the Ticket";
            else
                oEvent.EventDescription = oTicket.AssignToName + "(Team) is Assigned to the Ticket";
            new EventBL().Create(oEvent);
        }

        public void StatusChanged_ToContact(vw_SupportTicket oTicket, string Email, int LanguageId = 1)
        {
            string link = Utility.CommonFunction.GetSite_URL() + "/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=TicketView";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("StatusChanged_ToContact",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[TICKET_DESCRIPTION]]", oTicket.ProblemDescription);
                body = body.Replace("[[TICKET_STATUS]]", oTicket.StatusName);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);

                oEmailQueue.To = Email;
                oEmailQueue.ToName = oTicket.CompanyUserName;
                oEmailQueue.Subject = oEmailTemplate.Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                EmailQueueBL.SendMail(oEmailQueue, true);
            }
        }

        public void StatusChanged_ToOperator(vw_SupportTicket oTicket, string Email, string Name, string OldStatus, int LanguageId = 1)
        {
            string link = Utility.CommonFunction.GetSite_URL() + "/Account/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=TicketView";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("StatusChanged_ToOperator",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[TICKET_DESCRIPTION]]", oTicket.ProblemDescription);
                body = body.Replace("[[TICKET_STATUS]]", oTicket.StatusName);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);

                oEmailQueue.To = Email;
                oEmailQueue.ToName = oTicket.CompanyUserName;
                oEmailQueue.Subject = oEmailTemplate.Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                EmailQueueBL.SendMail(oEmailQueue, true);
            }
            Event oEvent = new Event();
            oEvent.TicketId = oTicket.TicketGuid;
            oEvent.EventDescription = Name + "(Operator) has Changed Status from (" + OldStatus + ") to (" + oTicket.StatusName + ")";
            new EventBL().Create(oEvent);
        }

        public void Closed_ToContact(vw_SupportTicket oTicket, string Email, int LanguageId = 1)
        {
            string link = Utility.CommonFunction.GetSite_URL() + "/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=TicketView";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("Closed_ToContact",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[TICKET_DESCRIPTION]]", oTicket.ProblemDescription);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);

                oEmailQueue.To = Email;
                oEmailQueue.ToName = oTicket.CompanyUserName;
                oEmailQueue.Subject = oEmailTemplate.Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                EmailQueueBL.SendMail(oEmailQueue, true);
               
            }
        }

        public void Closed_ToOperator(vw_SupportTicket oTicket, string Email, string Name, string OldStatus, int LanguageId = 1)
        {
            string link = Utility.CommonFunction.GetSite_URL() + "/Account/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=TicketView";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("Closed_ToOperator",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[TICKET_DESCRIPTION]]", oTicket.ProblemDescription);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);
                string Subject = oEmailTemplate.Subject;
                Subject = Subject.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);

                oEmailQueue.To = Email;
                oEmailQueue.ToName = oTicket.CompanyUserName;
                oEmailQueue.Subject = Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                EmailQueueBL.SendMail(oEmailQueue, true);
            }
            Event oEvent = new Event();
            oEvent.TicketId = oTicket.TicketGuid;
            oEvent.EventDescription = Name + "(Operator) has Changed Status from (" + OldStatus + ") to (" + oTicket.StatusName + ") in Support Ticket";
            new EventBL().Create(oEvent);
        }

        public void Reassignment(vw_SupportTicket oTicket, string CurrentLoggedOperator, string OldOperator, string Email, string NewOperator = null, int LanguageId = 1)
        {
            string link = Utility.CommonFunction.GetSite_URL() + "/Account/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=TicketView";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("Reassignment",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CURRENT_LOGGED_OPERATOR]]", CurrentLoggedOperator);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[TICKET_DESCRIPTION]]", oTicket.ProblemDescription);
                body = body.Replace("[[TICKET_OLD_OPERATORORTEAM]]", string.IsNullOrEmpty(OldOperator) ? "Unassigned" : OldOperator);
                body = body.Replace("[[TICKET_NEW_OPERATORORTEAM]]", NewOperator);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);
                string Subject = oEmailTemplate.Subject;
                Subject = Subject.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);

                oEmailQueue.To = Email;
                oEmailQueue.ToName = oTicket.CompanyUserName;
                oEmailQueue.Subject = Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                EmailQueueBL.SendMail(oEmailQueue, true);
            }
            Event oEvent = new Event();
            oEvent.TicketId = oTicket.TicketGuid;
            oEvent.EventDescription = OldOperator + "(Operator) has reassigned a Ticket to " + oTicket.AssignToName + " (Operator)";
            new EventBL().Create(oEvent);
        }

        public void Reassignment_ToTeam(vw_SupportTicket oTicket, string CurrentLoggedOperator, string OldOperator, List<int> UserIds, int LanguageId = 1)
        {
            string link = Utility.CommonFunction.GetSite_URL() + "/Account/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=TicketView";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("Reassignment",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CURRENT_LOGGED_OPERATOR]]", CurrentLoggedOperator);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[TICKET_DESCRIPTION]]", oTicket.ProblemDescription);
                body = body.Replace("[[TICKET_OLD_OPERATORORTEAM]]", string.IsNullOrEmpty(OldOperator) ? "Unassigned" : OldOperator);
                body = body.Replace("[[TICKET_NEW_OPERATORORTEAM]]", oTicket.AssignToName);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);
                string Subject = oEmailTemplate.Subject;
                Subject = Subject.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);

                oEmailQueue.Subject = Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                foreach (int item in UserIds)
                {
                    User optUser = new UserBL().GetById(item);
                    if (oEmailQueue.To == null)
                    {
                        oEmailQueue.To = optUser.Email;
                        oEmailQueue.ToName = optUser.Name;
                    }
                    else
                        oEmailQueue.CC = oEmailQueue.CC + optUser.Email + ",";
                }

                oEmailQueue.CC = oEmailQueue.CC.TrimEnd(',');
                EmailQueueBL.SendMail(oEmailQueue, true);
            }
            Event oEvent = new Event();
            oEvent.TicketId = oTicket.TicketGuid;
            oEvent.EventDescription = OldOperator + "(Operator) has reassigned a Ticket to " + oTicket.AssignToName + " (Team)";
            new EventBL().Create(oEvent);
        }

        public void CommmentCreation_ByContact_ToOperator(vw_SupportTicket oTicket, Comment oComments, string Email, string Name, int LanguageId = 1)
        {
            string link = Utility.CommonFunction.GetSite_URL() + "/Account/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=Comments";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("Comment_Creation_ByContact",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[COMMENT_TEXT]]", oComments.CommentText);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);

                oEmailQueue.To = Email;
                oEmailQueue.ToName = oTicket.CompanyUserName;
                oEmailQueue.Subject = oEmailTemplate.Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                EmailQueueBL.SendMail(oEmailQueue, true);
            }
            Event oEvent = new Event();
            oEvent.TicketId = oTicket.TicketGuid;
            oEvent.EventDescription = "A New Comment is Added by " + Name + " (User)";
            new EventBL().Create(oEvent);
        }

        public void CommmentCreation_ByContact_ToTeam(vw_SupportTicket oTicket, Comment oComments, List<int> UserIds, string Name, int LanguageId = 1)
        {
            string link = Utility.CommonFunction.GetSite_URL() + "/Account/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=Comments";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("Comment_Creation_ByContact",LanguageId);
            if (oEmailTemplate.IsEnable.Value)
            {
                EmailQueue oEmailQueue = new EmailQueue();

                string body = oEmailTemplate.Body;
                body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                body = body.Replace("[[COMMENT_TEXT]]", oComments.CommentText);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);

                oEmailQueue.Subject = oEmailTemplate.Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                foreach (int item in UserIds)
                {
                    User optUser = new UserBL().GetById(item);
                    if (oEmailQueue.To == null)
                    {
                        oEmailQueue.To = optUser.Email;
                        oEmailQueue.ToName = optUser.Name;
                    }
                    else
                        oEmailQueue.CC = oEmailQueue.CC + optUser.Email + ",";
                }

                oEmailQueue.CC = oEmailQueue.CC.TrimEnd(',');
                EmailQueueBL.SendMail(oEmailQueue, true);
            }
            Event oEvent = new Event();
            oEvent.TicketId = oTicket.TicketGuid;
            oEvent.EventDescription = "A New Comment is Added by " + Name + " (User)";
            new EventBL().Create(oEvent);
        }

        public void Creation_ByOperator(vw_SupportTicket oTicket, Comment oComments, string Email, string Name, bool IsPrivate, int LanguageId = 1)
        {
            string link = Utility.CommonFunction.GetSite_URL() + "/SupportTicket/View_Detail?id=" + oTicket.TicketGuid + "&tab=Comments";

            EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage("Comment_Creation_ByOperator",LanguageId);
            if (!IsPrivate)
            {
                if (oEmailTemplate.IsEnable.Value)
                {
                    EmailQueue oEmailQueue = new EmailQueue();

                    string body = oEmailTemplate.Body;
                    body = body.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);
                    body = body.Replace("[[CONTACT_DISPLAYNAME]]", oTicket.CompanyUserName);
                    body = body.Replace("[[TICKET_CATEGORY]]", oTicket.CategoryName);
                    body = body.Replace("[[COMMENT_TEXT]]", oComments.CommentText);
                    body = body.Replace("[[URL_TICKET_VIEW]]", link);
                    string Subject = oEmailTemplate.Subject;
                    Subject = Subject.Replace("[[TICKET_NUMBER]]", oTicket.TicketViewId);

                    oEmailQueue.To = Email;
                    oEmailQueue.ToName = oTicket.CompanyUserName;
                    oEmailQueue.Subject = Subject;
                    oEmailQueue.Body = body;
                    oEmailQueue.CreatedOn = DateTime.Now;

                    EmailQueueBL.SendMail(oEmailQueue, true);
                }
            }
            Event oEvent = new Event();
            oEvent.TicketId = oTicket.TicketGuid;
            oEvent.EventDescription = "A New Comment is Added by " + Name + " (Operator)";
            new EventBL().Create(oEvent);
        }

        #endregion

        #region CRUD Operations

        //update email template
        public void Update(EmailTemplate oEmailTemplate, User oUser)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oEmailTemplate.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oEmailTemplate.ModifiedOn = DateTime.Now;

                    ctx.Entry(oEmailTemplate).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //delete email template record
        public void Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    EmailTemplate oEmailTemplate = GetById(id);
                    ctx.Entry(oEmailTemplate).State = EntityState.Deleted;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
