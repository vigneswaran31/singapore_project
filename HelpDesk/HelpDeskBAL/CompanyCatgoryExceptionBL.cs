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
    public class CompanyCatgoryExceptionBL
    {
        #region Get Methods

        //Get CompanyCatgoryException data list in Grid Format.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.CompanyCategoryExceptions.AsQueryable();

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
                                    CompanyId = c.CompanyId,
                                    CategoryId = c.CategoryId,
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

        //Get CompanyCategoryException record detail by CompanyId
        public CompanyCategoryException GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.CompanyCategoryExceptions.Where(p => p.CompanyId == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get CompanyCategoryExceptions list record  by CompanyId
        public List<CompanyCategoryException> GetExceptionsByCompanyId(int CompanyId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.CompanyCategoryExceptions.Where(c => c.CompanyId == CompanyId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Operations

        //Create new CompanyCategoryExceptions record.
        public void Create(List<int> CategoryId, int CompanyId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    foreach (int id in CategoryId)
                    {
                        CompanyCategoryException oCompanyCategoryException = new CompanyCategoryException();
                        oCompanyCategoryException.CompanyId = CompanyId;
                        oCompanyCategoryException.CategoryId = id;
                        oCompanyCategoryException.CreatedOn = DateTime.Now;
                        ctx.CompanyCategoryExceptions.Add(oCompanyCategoryException);
                    }
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete Categories by Id from Company Category Exception.
        public void Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var lstCompanyException = ctx.CompanyCategoryExceptions.Where(p => p.CompanyId == id).ToList();
                    foreach (var obj in lstCompanyException)
                    {
                        ctx.CompanyCategoryExceptions.Remove(obj);
                    }
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
