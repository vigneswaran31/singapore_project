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
    public class SupportTeamBL
    {
        #region Get Methods

        //Get SupportTeam data list in Grid Format.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.SupportTeams.AsQueryable();

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
                                    TeamId = c.TeamId,
                                    Name = c.Name,
                                    Description = c.Description,
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
       
        //Get SupportTeam details by Id
        public SupportTeam GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.SupportTeams.Where(p => p.TeamId == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get All Support Teams record.
        public List<SupportTeam> GetAll()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.SupportTeams.Where(p => p.IsEnable == true).OrderBy(p => p.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #endregion

        #region CRUD Operations

        //Create new SupportTeam.
        public void Create(SupportTeam oSupportTeam)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oSupportTeam.ModifiedBy =  HttpContext.Current.User.Identity.Name;
                    oSupportTeam.ModifiedOn = DateTime.Now;
                    oSupportTeam.CreatedBy =  HttpContext.Current.User.Identity.Name;
                    oSupportTeam.CreatedOn = DateTime.Now;

                    ctx.SupportTeams.Add(oSupportTeam);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update Existing SupportTeam record.
        public void Update(SupportTeam oSupportTeam)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oSupportTeam.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oSupportTeam.ModifiedOn = DateTime.Now;
                    ctx.Entry(oSupportTeam).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete SupportTeam record by Id.
        public bool Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    SupportTeam oSupportTeam = ctx.SupportTeams.Where(p => p.TeamId == id).FirstOrDefault();
                    ctx.SupportTeams.Remove(oSupportTeam);
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
