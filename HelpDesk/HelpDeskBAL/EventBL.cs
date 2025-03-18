using HelpDeskEntity;
using HelpDeskEntity.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HelpDeskBAL
{
    public class EventBL
    {
        #region Get Methods

        //Get Event data list in Grid Format by Ticket Guid.
        public string GetGridData(GridSettings grid,Guid TicketGuid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.vw_Events.Where(e=>e.TicketGuid==TicketGuid).AsQueryable();


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
                                    EventId = c.EventId,
                                    TicketViewId = c.TicketViewId,
                                    EventDescription = c.EventDescription,
                                    CreatedOn = c.CreatedOn,
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

        //Create new Event record.
        public void Create(Event oEvent)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oEvent.CreatedOn = DateTime.Now;
                    ctx.Events.Add(oEvent);
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
