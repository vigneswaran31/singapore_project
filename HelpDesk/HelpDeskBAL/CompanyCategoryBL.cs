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
    public class CompanyCategoryBL
    {
        #region Get Methods

        //Get All CompanyCategorys record
        public List<CompanyCategory> GetAll()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.CompanyCategories.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Get CompanyCategory By Id
        public CompanyCategory GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.CompanyCategories.Where(c => c.Id == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Get Company Category grid data
        public string GetGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.CompanyCategories.AsQueryable();

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
                                    Name = c.Name,
                                    CreatedBy = c.CreatedBy,
                                    CreatedOn = c.CreatedOn,
                                    ModifiedBy  = c.ModifiedBy,
                                    ModifiedOn  = c.ModifiedOn,

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

        // Add Company Category record
        public void Create(CompanyCategory oCompanyCategory)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oCompanyCategory.CreatedBy = Utility.CommonFunction.GetLoginUserName();
                    oCompanyCategory.CreatedOn = DateTime.Now;
                    oCompanyCategory.ModifiedBy = Utility.CommonFunction.GetLoginUserName();
                    oCompanyCategory.ModifiedOn = DateTime.Now;

                    ctx.CompanyCategories.Add(oCompanyCategory);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Update Company Category record
        public void Update(CompanyCategory oCompanyCategory)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oCompanyCategory.ModifiedBy = Utility.CommonFunction.GetLoginUserName();
                    oCompanyCategory.ModifiedOn = DateTime.Now;

                    ctx.Entry(oCompanyCategory).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Delete Company Category record by Id
        public bool Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    CompanyCategory oCompanyCategory = ctx.CompanyCategories.Where(l => l.Id == id).FirstOrDefault();
                    ctx.CompanyCategories.Remove(oCompanyCategory);
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
