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
    public class SupportCategoryBL
    {
        #region Get Methods

        //Get SupportCategory data list in Grid Format.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.vw_SupportCategory.AsQueryable();
                    
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
                                    CategoryId = c.CategoryId,
                                    CategoryName = c.CategoryName,
                                    ParentCategoryName = c.ParentCategoryName,
                                    IsEnable = c.IsEnable,
                                    DefaultForNewTicket = c.DefaultForNewTicket,
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

        //Get SupportCategory data list for Doprdown by level.
        public List<sp_GetSupportCategoryList_Result> GetCategoryDDL(int level)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.sp_GetSupportCategoryList(level).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get SupportCategory data list by CompanyId.
        public List<sp_GetSupportCategoryList_Result> GetCompanyCategoryList(int CompanyId)
        {
            List<int> lstCompCatgryExption = new CompanyCatgoryExceptionBL().GetExceptionsByCompanyId(CompanyId).Select(c=>c.CategoryId).ToList();
            List<int> lstCompCatgryExpt = new SupportCategoryBL().GetAllSupportCategory().Where(c => lstCompCatgryExption.Contains(c.ParentId.Value)).Select(c=>c.CategoryId).ToList();
            if (lstCompCatgryExpt!=null &&lstCompCatgryExpt.Count>0)
            {
                lstCompCatgryExption = lstCompCatgryExption.Concat(lstCompCatgryExpt).ToList();
            }
            List<sp_GetSupportCategoryList_Result> lstCategories = new List<sp_GetSupportCategoryList_Result>();

            try
            {
                using (var ctx = new HelpDeskEntities())
                {

                    return ctx.sp_GetSupportCategoryList(2).Where(c => c.CategoryId != null && !lstCompCatgryExption.Contains(c.CategoryId.Value)).ToList();
                    //lstCategories = ctx.sp_GetSupportCategoryList().ToList();
                    //if (lstCompCatgryExption.Count!=0 && lstCompCatgryExption!=null)
                    //{
                    //    lstCategories.RemoveAll(c => lstCompCatgryExption.Contains(c.CategoryId.Value));
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get KBCategories details record by CategoryId.
        public SupportCategory GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.SupportCategories.Where(p => p.CategoryId == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get all SupportCategory list data 
        public List<SupportCategory> GetAllSupportCategory()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.SupportCategories.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SupportCategory GetDefaultSupportCategory()
        {
            try 
	        {	        
		        using(var ctx = new HelpDeskEntities())
                {
                    return GetAllSupportCategory().Where(p => p.DefaultForNewTicket == true).FirstOrDefault();
                }
	        }
	        catch (Exception ex)
	        {
		        throw;
	        }
        }

   
        #endregion

        #region CRUD Operations

        //Create new KBCategory.
        public void Create(SupportCategory oSupportCategory)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    if (oSupportCategory.ParentId == null)
                        oSupportCategory.ParentId = 0;

                    oSupportCategory.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oSupportCategory.ModifiedOn = DateTime.Now;
                    oSupportCategory.CreatedBy = HttpContext.Current.User.Identity.Name;
                    oSupportCategory.CreatedOn = DateTime.Now;

                    ctx.SupportCategories.Add(oSupportCategory);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update Existing KBCategory record.
        public void Update(SupportCategory oSupportCategory)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    if (oSupportCategory.ParentId == null)
                        oSupportCategory.ParentId = 0;

                    oSupportCategory.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oSupportCategory.ModifiedOn = DateTime.Now;
                    ctx.Entry(oSupportCategory).State = EntityState.Modified;
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
                using (var ctx = new HelpDeskEntities())
                {
                    SupportCategory oSupportCategory = ctx.SupportCategories.Where(p => p.CategoryId == id).FirstOrDefault();
                    ctx.SupportCategories.Remove(oSupportCategory);
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
