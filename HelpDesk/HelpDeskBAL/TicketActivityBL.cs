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
    public class TicketActivityBL
    {
        #region Get Methods

        // Get Ticket Activity By Id
        public TicketActivity GetById(int Id)
        {
            try
            {
                using(var ctx = new HelpDeskEntities())
                {
                    return ctx.TicketActivities.Where(p => p.Id == Id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Get Ticket Activity data in grid format
        public string GetGridData(GridSettings grid, int Id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    IQueryable<TicketActivity> query;

                    query = ctx.TicketActivities.Where(p => p.TicketId == Id).AsQueryable();

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
                                    TicketId = c.TicketId,
                                    Description = c.Description,
                                    Note = c.Note,
                                    FromDate = c.FromDate,
                                    ToDate = c.ToDate,
                                    SubtractFromContract = c.SubtractFromContract,
                                    CreatedOn = c.CreatedOn,
                                    CreatedBy = c.CreatedBy,
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

        //Create new Activity record.
        public void Create(TicketActivity oTicketActivity)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oTicketActivity.CreatedBy = Utility.CommonFunction.GetLoginUserName();
                    oTicketActivity.CreatedOn = DateTime.Now;
                    oTicketActivity.ModifiedBy = Utility.CommonFunction.GetLoginUserName();
                    oTicketActivity.ModifiedOn = DateTime.Now;
                    ctx.TicketActivities.Add(oTicketActivity);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update Existing Activity record.
        public void Update(TicketActivity oTicketActivity)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oTicketActivity.ModifiedBy = Utility.CommonFunction.GetLoginUserName();
                    oTicketActivity.ModifiedOn = DateTime.Now;
                    ctx.Entry(oTicketActivity).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete Activity record by Id
        public bool Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    TicketActivity oTicketActivity = ctx.TicketActivities.Where(p => p.Id == id).FirstOrDefault();
                    ctx.TicketActivities.Remove(oTicketActivity);
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
