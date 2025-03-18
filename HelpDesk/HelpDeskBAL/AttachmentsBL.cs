using HelpDeskEntity;
using HelpDeskEntity.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Utility;

namespace HelpDeskBAL
{
    public class AttachmentsBL
    {
        #region Get Methods

        //Get Attachment List by Type and Id.
        public List<Attachment> GetByTypeAndId(int LinkType, int LinkId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Attachments.Where(p => p.LinkType == LinkType && p.LinkId == LinkId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Attachment List by Id.
        public Attachment GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Attachments.Where(p => p.AttachmentId == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region CRUD Operations

        //Upload New Attachment on the system.
        public void Create(Attachment attachment)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    attachment.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    attachment.ModifiedOn = DateTime.Now;
                    attachment.CreatedBy = HttpContext.Current.User.Identity.Name;
                    attachment.CreatedOn = DateTime.Now;

                    ctx.Attachments.Add(attachment);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete Attachment from server by Attachment Id.
        public void Delete(int id, string ImagePath)// used when dropzone in popup
        {
            try
            {
                Attachment attachment = GetById(id);

                string imgDirectory = HttpContext.Current.Server.MapPath(ImagePath);
                string filePath = Path.Combine(imgDirectory, attachment.FilePath);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                filePath = Path.Combine(imgDirectory, attachment.FilePath.Remove(attachment.FilePath.LastIndexOf('.')) + "_thumb"
                                            + Path.GetExtension(attachment.FilePath));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                using (var ctx = new HelpDeskEntities())
                {
                    Attachment oAttachment = ctx.Attachments.Where(p => p.AttachmentId == id).FirstOrDefault();
                    ctx.Attachments.Remove(oAttachment);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteByLinkId(int LinkId, string ImagePath) // used when delete entity
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var lstAttachments = ctx.Attachments.Where(p => p.LinkId == LinkId).ToList();

                    foreach (var obj in lstAttachments)
                        Delete(obj.AttachmentId, ImagePath);

                    string imgDirectory = HttpContext.Current.Server.MapPath(ImagePath);

                    if (System.IO.Directory.Exists(imgDirectory + "/" + LinkId))
                        System.IO.Directory.Delete(imgDirectory + "/" + LinkId);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveImage(int id) // used when dropzone in page
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    Attachment oAttachment = ctx.Attachments.Where(p => p.AttachmentId == id).FirstOrDefault();
                    ctx.Attachments.Remove(oAttachment);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Send Mail

        //Send mail Notification when  Attachment is uploaded
        public void SendAttachmentMail(string key, vw_SupportTicket vwTicket, string link, string FileNames, int LanguageId = 1)
        {
            try
            {
                EmailQueue oEmailQueue = new EmailQueue();
                EmailTemplate oEmailTemplate = new EmailTemplateBL().GetByKeyLanguage(key,LanguageId);

                string body = oEmailTemplate.Body;

                body = body.Replace("[[TICKET_NUMBER]]", vwTicket.TicketId.ToString());
                body = body.Replace("[[CONTACT_DISPLAYNAME]]", vwTicket.CompanyUserName);
                body = body.Replace("[[TICKET_CATEGORY]]", vwTicket.CategoryName);
                body = body.Replace("[[TICKET_DESCRIPTION]]", vwTicket.ProblemDescription);
                body = body.Replace("[[FILE_NAME]]", FileNames);
                body = body.Replace("[[URL_TICKET_VIEW]]", link);

                string Subject = oEmailTemplate.Subject;
                Subject = Subject.Replace("[[TICKET_NUMBER]]", vwTicket.TicketViewId);

                oEmailQueue.To = vwTicket.Email;
                oEmailQueue.ToName = vwTicket.CompanyUserName;
                oEmailQueue.Subject = Subject;
                oEmailQueue.Body = body;
                oEmailQueue.CreatedOn = DateTime.Now;

                EmailQueueBL.SendMail(oEmailQueue, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
