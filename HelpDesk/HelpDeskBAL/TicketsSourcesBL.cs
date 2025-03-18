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
    public class TicketsSourcesBL
    {
        #region Get Methods

        //Get Ticket Source data list in Grid Format.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.TicketsSources.AsQueryable();

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
                                    Description = c.Description,
                                    DefaultForMail = c.DefaultForMail,
                                    DefaultForTicket = c.DefaultForTicket,
                                    Action = c.Id,
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

        //Get All TicketSource record
        public List<TicketsSource> GetAllTicketSources()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.TicketsSources.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Get Default Ticket Source for Mail
        public TicketsSource GetDefaultTicketSourceForMail()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return GetAllTicketSources().Where(p => p.DefaultForMail == true).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Get Default Ticket Source for Ticket
        public TicketsSource GetDefaultTicketSourceForTicket()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return GetAllTicketSources().Where(p => p.DefaultForTicket == true).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Get Ticket Source object by Id
        public TicketsSource GetById(int id)
        {
            try
            {
                using(var ctx = new HelpDeskEntities())
                {
                    return ctx.TicketsSources.Where(p => p.Id == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Operations

        //Create new TicketSource  record.
        public void Create(TicketsSource oTicketsSource)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    // Update other DefaultForMail field false when this is check
                    if (oTicketsSource.DefaultForMail == true)
                        ctx.Database.ExecuteSqlCommand("UPDATE TicketsSources SET DefaultForMail = 0");

                    // Update other DefaultForTicket field false when this is check
                    if (oTicketsSource.DefaultForTicket == true)
                        ctx.Database.ExecuteSqlCommand("UPDATE TicketsSources SET DefaultForTicket = 0");

                    ctx.TicketsSources.Add(oTicketsSource);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update TicketSource record.
        public void Update(TicketsSource oTicketsSource)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    // Update other DefaultForMail field false when this is check
                    if (oTicketsSource.DefaultForMail == true)
                        ctx.Database.ExecuteSqlCommand("UPDATE TicketsSources SET DefaultForMail = 0 WHERE Id <> " + oTicketsSource.Id);

                    // Update other DefaultForTicket field false when this is check
                    if (oTicketsSource.DefaultForTicket == true)
                        ctx.Database.ExecuteSqlCommand("UPDATE TicketsSources SET DefaultForTicket = 0 WHERE Id <> " + oTicketsSource.Id);

                    ctx.Entry(oTicketsSource).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete TicketSource by Id
        public bool Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    TicketsSource oTicketsSource = ctx.TicketsSources.Where(p => p.Id == id).FirstOrDefault();
                    ctx.TicketsSources.Remove(oTicketsSource);
                    ctx.SaveChanges();
                    return true;
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
