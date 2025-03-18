using HelpDeskEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskBAL
{
    public class SupportTeamPermissionBL
    {
        #region Get Methods

        //Get all SupportTeamPermission list by Team Id.
        public List<SupportTeamPermission> GetByTeamId(int TeamId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.SupportTeamPermissions.Where(p => p.TeamId == TeamId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Operations

        //Create new SupportTeamPermission record.
        public void Create(List<SupportTeamPermission> lstSupportTeamPermission)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    Delete(lstSupportTeamPermission[0].TeamId);
                    foreach (SupportTeamPermission p in lstSupportTeamPermission)
                    {
                        p.CreatedOn = DateTime.Now;
                        ctx.SupportTeamPermissions.Add(p);
                    }
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete SupportTeamPermission record by TeamId
        public void Delete(int TeamId)
        {
            try
            {
                using(var ctx = new HelpDeskEntities())
                {
                    var lstTeamPermission = ctx.SupportTeamPermissions.Where(p => p.TeamId == TeamId).ToList();
                    foreach(var obj in lstTeamPermission)
                    {
                        ctx.SupportTeamPermissions.Remove(obj);
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
