using HelpDeskEntity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HelpDeskBAL
{
    public class TicketStatusBL
    {
        #region Get Methods

        //Get TicketStatus Details by Id.
        public TicketStatu GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.TicketStatus.Where(c => c.TicketStatusId == id).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get all TicketStatus list record.
        public List<TicketStatu> GetStatus()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.TicketStatus.Where(c => c.IsEnable == true).OrderBy(c=>c.OrderByNo).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get Default Status record for New Ticket.
        public TicketStatu GetDefaultStatusForNewTicket()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.TicketStatus.Where(c => c.DefaultForNewTicket == true).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get all Open Ticket Status list record.
        public List<TicketStatu> GetAllOpenStatus()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.TicketStatus.Where(c => c.IsEnable == true && c.IsClosedStatus == false).OrderBy(c => c.OrderByNo).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get Ticket Status data list in Grid Format.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.TicketStatus.AsQueryable();


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
                                    TicketStatusId = c.TicketStatusId,
                                    StatusName = c.StatusName,
                                    IsEnable = c.IsEnable,
                                    DefaultForNewTicket = c.DefaultForNewTicket,
                                    IsClosedStatus= c.IsClosedStatus,
                                    IconColor=c.IconColor,
                                    OrderByNo=c.OrderByNo,
                                    CreatedOn = c.CreatedOn,
                                    CreatedBy = c.CreatedBy,
                                    ModifiedOn = c.ModifiedOn,
                                    ModifiedBy = c.ModifiedBy,
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

        //Create new TicketStatus record.
        public void Create(TicketStatu oTicketStatu)
        {
            try
            {
                if (oTicketStatu.DefaultForNewTicket == true)
                {
                    ResetDefaultPriority();
                }
                using (var ctx = new HelpDeskEntities())
                {

                    oTicketStatu.CreatedBy = HttpContext.Current.User.Identity.Name;
                    oTicketStatu.CreatedOn = DateTime.Now;
                    oTicketStatu.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oTicketStatu.ModifiedOn = DateTime.Now;
                    ctx.TicketStatus.Add(oTicketStatu);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update Existing TicketStatus record.
        public void Update(TicketStatu oTicketStatu)
        {
            try
            {
                if (oTicketStatu.DefaultForNewTicket == true)
                {
                    ResetDefaultPriority();
                }
                using (var ctx = new HelpDeskEntities())
                {
                    oTicketStatu.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oTicketStatu.ModifiedOn = DateTime.Now;
                    ctx.Entry(oTicketStatu).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete TicketStatus record by Id
        public void Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    TicketStatu oTicketStatu = ctx.TicketStatus.Where(c => c.TicketStatusId == id).FirstOrDefault();
                    ctx.TicketStatus.Remove(oTicketStatu);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void ResetDefaultPriority()
        {
            using (var ctx = new HelpDeskEntities())
            {
                List<TicketStatu> lstTicketStatu = ctx.TicketStatus.Where(t => t.DefaultForNewTicket == true).ToList();
                if (lstTicketStatu.Count > 0 && lstTicketStatu != null)
                {
                    lstTicketStatu.ForEach(a => a.DefaultForNewTicket = false);
                    ctx.SaveChanges();
                }

            }
        }   
        #endregion
    }
}
