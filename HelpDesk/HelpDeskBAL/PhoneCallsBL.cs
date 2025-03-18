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

namespace HelpDeskBAL
{
    public class PhoneCallsBL
    {
        #region Get Methods

        public string GetGridDataByTicketId(GridSettings grid, int TicketId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.PhoneCalls.Include("User").Where(p => p.TicketId == TicketId).AsQueryable();

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
                                    timeSpan = TimeSpan.FromMinutes(138.34),
                                    Id = c.Id,
                                    Date = c.Date,
                                    Phone = c.Phone,
                                    Name = c.User.Name,
                                    Comment = c.Comment,
                                    CallTime = TimeSpan.FromMinutes(c.CallTime.Value).Hours + " hours and " + TimeSpan.FromMinutes(c.CallTime.Value).Minutes + " minutes",
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

        public DateTime GetTotalTimeOfPhoneCalls(int TicketId)
        {
            try
            {
                int Minutes = 0;
                using(var ctx = new HelpDeskEntities())
                {
                    var lstPhoneCalls = ctx.PhoneCalls.Where(p => p.TicketId == TicketId).ToList();
                    foreach(var obj in lstPhoneCalls)
                    {
                        Minutes = Minutes + obj.CallTime.Value;
                    }
                   // return TimeSpan.FromMinutes(Minutes).Hours + " : " + TimeSpan.FromMinutes(Minutes).Minutes;
                    var dateNow = DateTime.Now;
                    var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, TimeSpan.FromMinutes(Minutes).Hours, TimeSpan.FromMinutes(Minutes).Minutes,0);
                    return date;
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
