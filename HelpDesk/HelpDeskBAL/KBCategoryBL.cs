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
    public class KBCategoryBL
    {
        #region Get Methods

        //Get KBCategories data list in Grid Format.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.vw_KBCategory.AsQueryable();

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
                                    KBCategoryId = c.KBCategoryId,
                                    CategoryName = c.CategoryName,
                                    ParentCategoryName = c.ParentCategoryName,
                                    IsEnable = c.IsEnable,
                                    CreatedBy = c.CreatedBy,
                                    CreatedOn = c.CreatedOn,
                                    ModifiedBy = c.ModifiedBy,
                                    ModifiedOn = c.ModifiedOn
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

        //Get KBCategories data list for Dropdown by level.
        public List<sp_GetCategoryList_Result> GetCategoryDDL(int level)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.sp_GetCategoryList(level).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get KBCategories details by KBCategoryId.
        public KBCategory GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.KBCategories.Where(p => p.KBCategoryId == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<KBCategory> GetAllParents()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.KBCategories.Where(p => p.ParentId == 0).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get all KBCategory list records
        public List<KBCategory> GetAll()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.KBCategories.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get all Child KBCategory by Parent Id
        public List<KBCategory> GetChildsByParentId(int ParentId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.KBCategories.Where(p => p.ParentId == ParentId && p.IsEnable == true).OrderBy(p => p.CategoryName).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Operations

        //Create new KBCategory.
        public void Create(KBCategory oCategory)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oCategory.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oCategory.ModifiedOn = DateTime.Now;
                    oCategory.CreatedBy =  HttpContext.Current.User.Identity.Name;
                    oCategory.CreatedOn = DateTime.Now;

                    ctx.KBCategories.Add(oCategory);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update Existing KBCategory record.
        public void Update(KBCategory oCategory)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oCategory.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oCategory.ModifiedOn = DateTime.Now;
                    ctx.Entry(oCategory).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete KBCategory record by Id.
        public bool Delete(int id)
        {
            try
            {
                using(var ctx = new HelpDeskEntities())
                {
                    KBCategory ParentCategory = ctx.KBCategories.Where(p => p.ParentId == id).FirstOrDefault();
                    if (ParentCategory == null)
                    {
                        KBCategory oCategory = ctx.KBCategories.Where(p => p.KBCategoryId == id).FirstOrDefault();
                        ctx.KBCategories.Remove(oCategory);
                        ctx.SaveChanges();
                        return true;
                    }
                    else
                        return false;
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
