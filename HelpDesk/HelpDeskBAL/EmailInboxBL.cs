using HelpDeskEntity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskBAL
{
    public class EmailInboxBL
    {
        #region Get Methods

        //Get Email Inbox by Id
        public EmailInbox GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.EmailInboxes.Where(p => p.Id == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Email Inbox data in grid format
        public string GetGridData(GridSettings grid, string Id = null)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    IQueryable<EmailInbox> query;

                    if (Id != null)
                        query = ctx.EmailInboxes.Where(p => p.LinkedToTicketId == Id && p.Processed == true).AsQueryable();
                    else
                        query = ctx.EmailInboxes.AsQueryable();

                    int count;
                    var data = query.GridCommonSettings(grid, out count);

                    var result = new
                    {
                        total = (int)Math.Ceiling((double)count / grid.PageSize),
                        page = grid.PageIndex,
                        records = count,
                        rows = (from c in data
                                select new
                                {
                                    Id = c.Id,
                                    Date = c.Date,
                                    FromAddress = c.FromAddress,
                                    Subject = c.Subject,
                                    Body = c.Body,
                                    Processed = c.Processed,
                                    LinkedToTicketId = c.LinkedToTicketId,
                                    Action = c.Id
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

        // Get UnProcessed Email Inbox data in grid format
        public string GetUnProcessedGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    IQueryable<EmailInbox> query;

                    query = ctx.EmailInboxes.Where(p => p.Processed == false).AsQueryable();

                    int count;
                    var data = query.GridCommonSettings(grid, out count);

                    var result = new
                    {
                        total = (int)Math.Ceiling((double)count / grid.PageSize),
                        page = grid.PageIndex,
                        records = count,
                        rows = (from c in data
                                select new
                                {
                                    Id = c.Id,
                                    Date = c.Date,
                                    FromAddress = c.FromAddress,
                                    Subject = c.Subject,
                                    Body = c.Body,
                                    Action = c.Id
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

        #endregion

        #region CRUD Operations

        // update Email Inbox 
        public void Update(EmailInbox oEmailInbox)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    ctx.Entry(oEmailInbox).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // update Email Inbox by Id
        public void Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    EmailInbox oEmailInbox = ctx.EmailInboxes.Where(p => p.Id == id).FirstOrDefault();
                    ctx.EmailInboxes.Remove(oEmailInbox);
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
