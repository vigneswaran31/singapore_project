using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpDeskEntity;
using HelpDeskEntity.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data;
using System.Web;
namespace HelpDeskBAL
{
    public class TicketPriorityBL
    {
        #region Get Methods

        //Get TicketPriority details record by Id.
        public TicketPriority GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.TicketPriorities.Where(c => c.TicketPriorityId == id).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<TicketPriority> GetByAll()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.TicketPriorities.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        //Get TicketPriority data list in Grid Format by UserType.
        public string GetGridData(GridSettings grid,int UserType)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.TicketPriorities.Where(t=>t.Type==UserType).AsQueryable();


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
                                    TicketPriorityId = c.TicketPriorityId,
                                    Description = c.Description,
                                    Type = c.Type,
                                    IsEnable = c.IsEnable,
                                    DefaultForNewTicket=c.DefaultForNewTicket,
                                    OrderByNo = c.OrderByNo,
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

        //Get TicketPriority list record by UserType.
        public List<TicketPriority> GetPrioritiesByType(int Usertype)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.TicketPriorities.Where(c => c.Type == Usertype && c.IsEnable == true).OrderBy(c => c.OrderByNo).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get Default TicketPriority detail by UserType.
        public TicketPriority GetDefaultPriorityByType(int UserType)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.TicketPriorities.Where(c => c.Type == UserType && c.DefaultForNewTicket == true).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region CRUD Operations

        //Create new TicketPriority record.
        public void Create(TicketPriority oTicketPriority)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    if (oTicketPriority.DefaultForNewTicket==true)
                    {
                        ResetDefaultPriority(oTicketPriority.Type);
                    }
                    oTicketPriority.CreatedBy = HttpContext.Current.User.Identity.Name;
                    oTicketPriority.CreatedOn = DateTime.Now;
                    oTicketPriority.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oTicketPriority.ModifiedOn = DateTime.Now;
                    ctx.TicketPriorities.Add(oTicketPriority);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update Existing TicketPriority record.
        public void Update(TicketPriority oTicketPriority)
        {
            try
            {
                if (oTicketPriority.DefaultForNewTicket == true)
                {
                    ResetDefaultPriority(oTicketPriority.Type);
                }
                using (var ctx = new HelpDeskEntities())
                {
                    oTicketPriority.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oTicketPriority.ModifiedOn = DateTime.Now;
                    ctx.Entry(oTicketPriority).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete TicketPriority record by Id
        public void Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    TicketPriority oTicketPriority = ctx.TicketPriorities.Where(c => c.TicketPriorityId == id).FirstOrDefault();
                    ctx.TicketPriorities.Remove(oTicketPriority);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void ResetDefaultPriority(int UserTypeid)
        {
            using (var ctx=new HelpDeskEntities())
            {
                List<TicketPriority> lstTicketPriority = ctx.TicketPriorities.Where(t => t.Type == UserTypeid && t.DefaultForNewTicket == true).ToList();
                if (lstTicketPriority.Count>0 &&lstTicketPriority!=null)
                {
                    lstTicketPriority.ForEach(a => a.DefaultForNewTicket = false);
                    ctx.SaveChanges();
                }
               
            }
        }
        #endregion
    }
}
