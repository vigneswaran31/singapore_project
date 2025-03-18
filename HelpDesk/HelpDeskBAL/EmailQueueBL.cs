using HelpDeskEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace HelpDeskBAL
{
    public class EmailQueueBL
    {
        public static int Keep_In_DB_MailCount = 5;
        public static int Max_Attempt = 5;

        #region Get Methods

        //Get All Active/PendingToSend from Email Queue for sending mail
        public static List<EmailQueue> Get_All_Active_EmailQueue()
        {
            using (var ctx = new HelpDeskEntities())
            {
                return ctx.EmailQueues.Where(c => (
                                                    (c.CreatedOn < c.ExpiredOn || c.ExpiredOn == null) &&
                                                    (c.NoOfRetry == 0 || (c.NoOfRetry > 0 && c.NoOfRetry < Max_Attempt && c.FailureDate != null))
                                                  )).ToList();
            }
        }

        #endregion

        #region Operations

        //Process Mail Sending of items in Email Queue table
        public static void ProcessEmailQueue()
        {
            foreach (EmailQueue oEmailQueue in Get_All_Active_EmailQueue())
                ProcessEmailQueueItem(oEmailQueue);
        }

        public static void DeleteEmail()
        {
            List<EmailQueue> lstEmail = Get_All_Active_EmailQueue();

            using (var ctx = new HelpDeskEntities())
            {
                if (lstEmail.Count > Keep_In_DB_MailCount)
                {
                    int NoOfDeleteMail = lstEmail.Count - Keep_In_DB_MailCount;
                    foreach (EmailQueue oEmail in lstEmail.Take(NoOfDeleteMail).ToList())
                    {
                        if (oEmail.NoOfRetry > 0)
                        {
                            ctx.Entry(oEmail).State = EntityState.Deleted;
                            ctx.SaveChanges();
                        }
                    }
                }
            }
        }

        //Process Single Item (actual mail sending)
        public static void ProcessEmailQueueItem(EmailQueue oQueue, System.Net.Mail.Attachment attachment = null)
        {
            try
            {
                if (oQueue.NoOfRetry < Max_Attempt)
                {
                    oQueue.NoOfRetry = oQueue.NoOfRetry + 1;
                    Update(oQueue);

                    MailMessage message = new MailMessage();
                    if (!string.IsNullOrEmpty(oQueue.From))
                        message.From = new MailAddress(oQueue.From, oQueue.FromName == null ? oQueue.From : oQueue.FromName, Encoding.UTF8);

                    message.To.Add(new MailAddress(oQueue.To, oQueue.ToName == "" ? oQueue.To : oQueue.ToName, Encoding.UTF8));

                    //Add CC Address
                    if (!string.IsNullOrWhiteSpace(oQueue.CC))
                    {
                        string[] LstCCAddress = oQueue.CC.Split(';');
                        foreach (var CCAddress in LstCCAddress)
                        {
                            if (!string.IsNullOrWhiteSpace(CCAddress))
                                message.CC.Add(CCAddress);
                        }
                    }

                    //Add BCC Address
                    if (!string.IsNullOrWhiteSpace(oQueue.BCC))
                    {
                        string[] LstBCCAddress = oQueue.BCC.Split(';');
                        foreach (var BCCAddress in LstBCCAddress)
                        {
                            if (!string.IsNullOrWhiteSpace(BCCAddress))
                                message.Bcc.Add(BCCAddress);
                        }
                    }

                    message.Subject = oQueue.Subject;
                    message.SubjectEncoding = Encoding.UTF8;
                    message.IsBodyHtml = true;
                    message.Body = oQueue.Body;

                    message.BodyEncoding = Encoding.UTF8;
                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Timeout = 1000000;
                    smtpClient.Send(message);
                }
            }
            catch (Exception err)
            {
                oQueue.FailureReason = err.Message;
                while (err.InnerException != null)
                {
                    oQueue.FailureReason += err.InnerException.Message.ToString();
                    err = err.InnerException;
                }
                oQueue.FailureDate = DateTime.Now;
                Update(oQueue);
            }
        }

        //Add Mail in Queue , Send immediatly if requested
        public static void SendMail(EmailQueue oQueue, bool isImmediate = false)
        {
            using (var ctx = new HelpDeskEntities())
            {
                oQueue.CreatedOn = DateTime.Now;
                oQueue.NoOfRetry = 0;

                ctx.EmailQueues.Add(oQueue);
                ctx.SaveChanges();

                if (isImmediate)
                    ProcessEmailQueueItem(oQueue);
            }
        }

        //Update Email Queue Item
        public static void Update(EmailQueue oEmail)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew,
              new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }
              ))
            {
                try
                {
                    using (var ctx = new HelpDeskEntities())
                    {
                        ctx.Configuration.ValidateOnSaveEnabled = false;
                        ctx.Entry(oEmail).State = EntityState.Modified;
                        ctx.SaveChanges();
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }

        //Delete Email Queue Item
        public static void Delete(EmailQueue oEmail)
        {
            using (var ctx = new HelpDeskEntities())
            {
                ctx.Entry(oEmail).State = EntityState.Deleted;
                ctx.SaveChanges();
            }
        }

        public static void SendBulkMails(List<EmailQueue> oEmail)
        {
            using (var scope1 = new TransactionScope(TransactionScopeOption.RequiresNew,
            new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead }
            ))
            {
                try
                {
                    using (var ctx1 = new HelpDeskEntities())
                    {
                        ctx1.Database.Connection.Open();
                        foreach (var objtblEmailQueue in oEmail)
                        {
                            objtblEmailQueue.CreatedOn = DateTime.Now;
                            ctx1.EmailQueues.Add(objtblEmailQueue);
                        }
                        ctx1.SaveChanges();
                    }
                    scope1.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    scope1.Dispose();
                }
            }
        }

        #endregion
    }
}
