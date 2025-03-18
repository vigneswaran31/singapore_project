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
    public class AutoAssignBL
    {
        #region Get Methods

        //Get Auto Assigns data list in Grid Format.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.vw_AutoAssign.AsQueryable();

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
                                    CompanyName = c.CompanyName,
                                    CategoryName = c.CategoryName,
                                    Name = c.Name,
                                    TeamName = c.TeamName,
                                    CreatedBy = c.CreatedBy,
                                    CreatedOn = c.CreatedOn,
                                    ModifiedBy = c.ModifiedBy,
                                    ModifiedOn = c.ModifiedOn,
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

        //Get Companies data list in Grid Format.
        public string CompanyGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.vw_AutoAssign.Where(p => p.CategoryId == null).AsQueryable();

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
                                    CompanyName = c.CompanyName,
                                    CategoryName = c.CategoryName,
                                    Name = c.Name,
                                    TeamName = c.TeamName,
                                    CreatedBy = c.CreatedBy,
                                    CreatedOn = c.CreatedOn,
                                    ModifiedBy = c.ModifiedBy,
                                    ModifiedOn = c.ModifiedOn,
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

        //Get Categories data list in Grid Format.
        public string CategoryGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.vw_AutoAssign.Where(p => p.CompanyId == null).AsQueryable();

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
                                    CompanyName = c.CompanyName,
                                    CategoryName = c.CategoryName,
                                    Name = c.Name,
                                    TeamName = c.TeamName,
                                    CreatedBy = c.CreatedBy,
                                    CreatedOn = c.CreatedOn,
                                    ModifiedBy = c.ModifiedBy,
                                    ModifiedOn = c.ModifiedOn,
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

        //Get Auto Assign record detail by id
        public AutoAssign GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.AutoAssigns.Where(p => p.Id == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Auto Assign record User by CompanyId and UserId.
        public AutoAssign GetByCompanyAndUser(int CompanyId, int UserId)
        {
            try
            {
                AutoAssign oAutoAssign = new AutoAssign();
                using (var ctx = new HelpDeskEntities())
                {
                    oAutoAssign = ctx.AutoAssigns.Where(p => p.CompanyId == CompanyId && p.OperatorId == UserId).FirstOrDefault();
                    if (oAutoAssign == null)
                        oAutoAssign = ctx.AutoAssigns.Where(p => p.CompanyId == CompanyId && p.SupportTeamId == UserId).FirstOrDefault();

                    return oAutoAssign;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Auto Assign record User by CategoryId and UserId.
        public AutoAssign GetByCategoryAndUser(int CategoryId, int UserId)
        {
            try
            {
                AutoAssign oAutoAssign = new AutoAssign();
                using (var ctx = new HelpDeskEntities())
                {
                    oAutoAssign = ctx.AutoAssigns.Where(p => p.CategoryId == CategoryId && p.OperatorId == UserId).FirstOrDefault();
                    if (oAutoAssign == null)
                        oAutoAssign = ctx.AutoAssigns.Where(p => p.CategoryId == CategoryId && p.SupportTeamId == UserId).FirstOrDefault();

                    return oAutoAssign;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Operations

        //Create new AutoAssign record.
        public void Create(AutoAssign oAutoAssign)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oAutoAssign.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oAutoAssign.ModifiedOn = DateTime.Now;
                    oAutoAssign.CreatedBy = HttpContext.Current.User.Identity.Name;
                    oAutoAssign.CreatedOn = DateTime.Now;

                    ctx.AutoAssigns.Add(oAutoAssign);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update Existing AutoAssign record.
        public void Update(AutoAssign oAutoAssign)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oAutoAssign.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oAutoAssign.ModifiedOn = DateTime.Now;
                    ctx.Entry(oAutoAssign).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete AutoAssign record by Id
        public void Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    AutoAssign oAutoAssign = ctx.AutoAssigns.Where(p => p.Id == id).FirstOrDefault();
                    ctx.AutoAssigns.Remove(oAutoAssign);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Auto Assign OperatorId by CompanyId and CategoryId.
        public int AutoAssignOperator(int CompanyId, int CategoryId)
        {
            try
            {
                int OperatorId = 0;
                List<AutoAssign> lstCompanyAutoAssign = new List<AutoAssign>();
                List<AutoAssign> lstCategoryAutoAssign = new List<AutoAssign>();
                using (var ctx = new HelpDeskEntities())
                {
                    lstCompanyAutoAssign = ctx.AutoAssigns.Where(p => p.CompanyId == CompanyId && p.OperatorId != null).ToList();
                    if (lstCompanyAutoAssign.Count > 0)
                        OperatorId = lstCompanyAutoAssign.FirstOrDefault().OperatorId.Value;
                    else
                    {
                        lstCategoryAutoAssign = ctx.AutoAssigns.Where(p => p.CategoryId == CategoryId && p.OperatorId != null).ToList();
                        if (lstCategoryAutoAssign.Count > 0)
                            OperatorId = lstCategoryAutoAssign.FirstOrDefault().OperatorId.Value;
                    }
                }

                return OperatorId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Auto Assign SupportTeamId by CompanyId and CategoryId.
        public int AutoAssignSupportTeam(int CompanyId, int CategoryId)
        {
            try
            {
                int SupportTeamId = 0;
                List<AutoAssign> lstCompanyAutoAssign = new List<AutoAssign>();
                List<AutoAssign> lstCategoryAutoAssign = new List<AutoAssign>();
                using (var ctx = new HelpDeskEntities())
                {
                    lstCompanyAutoAssign = ctx.AutoAssigns.Where(p => p.CompanyId == CompanyId && p.SupportTeamId != null).ToList();
                    if (lstCompanyAutoAssign.Count > 0)
                        SupportTeamId = lstCompanyAutoAssign.FirstOrDefault().SupportTeamId.Value;
                    else
                    {
                        lstCategoryAutoAssign = ctx.AutoAssigns.Where(p => p.CategoryId == CategoryId && p.SupportTeamId != null).ToList();
                        if (lstCategoryAutoAssign.Count > 0)
                            SupportTeamId = lstCategoryAutoAssign.FirstOrDefault().SupportTeamId.Value;
                    }
                }

                return SupportTeamId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
