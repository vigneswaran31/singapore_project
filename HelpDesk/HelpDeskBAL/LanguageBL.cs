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
    public class LanguageBL
    {
        #region Get Methods

        //Get All Languages record
        public List<Language> GetAllLanguages()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Languages.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Language GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Languages.Where(c => c.Id == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.Languages.AsQueryable();

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

        #endregion

        #region CRUD Operations

        public void Create(Language oLanguage)
        {
            try
            {
                using(var ctx=new HelpDeskEntities())
                {
                    ctx.Languages.Add(oLanguage);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(Language oLanguage)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    ctx.Entry(oLanguage).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    Language oLanguage = ctx.Languages.Where(l => l.Id == id).FirstOrDefault();
                    ctx.Languages.Remove(oLanguage);
                    ctx.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        #endregion
    }
}
