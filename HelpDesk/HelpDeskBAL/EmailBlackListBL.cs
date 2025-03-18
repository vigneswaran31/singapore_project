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
    public class EmailBlackListBL
    {
        #region Get Methods

        // Get Email Blacklist data in grid format
        public string GetGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    IQueryable<EmailBlackList> query;

                    query = ctx.EmailBlackLists.AsQueryable();

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
                                    MailAddress = c.MailAddress,
                                    DeleteOnRead = c.DeleteOnRead,
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

        // Get EmailBlackList by Id
        public EmailBlackList GetById(int id)
        {
            try
            {
                using(var ctx = new HelpDeskEntities())
                {
                    return ctx.EmailBlackLists.Where(p => p.Id == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Operations

        //Create new BlackList record.
        public void Create(EmailBlackList oEmailBlackList)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    ctx.EmailBlackLists.Add(oEmailBlackList);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update Existing BlackList record.
        public void Update(EmailBlackList oEmailBlackList)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    ctx.Entry(oEmailBlackList).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete BlackList record by Id
        public bool Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    EmailBlackList oEmailBlackList = ctx.EmailBlackLists.Where(p => p.Id == id).FirstOrDefault();
                    ctx.EmailBlackLists.Remove(oEmailBlackList);
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
