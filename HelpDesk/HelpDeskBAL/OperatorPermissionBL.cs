using HelpDeskEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HelpDeskBAL
{
    public class OperatorPermissionBL
    {
        #region Get Methods

        //Get Operator Permission list by OperatorId
        public List<OperatorPermission> GetByOperatorId(int OperatorId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.OperatorPermissions.Where(p => p.OperatorId == OperatorId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Nullable<int>> GetOperatorPermissionsByOptId(int OperatorId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.sp_GetOperatorPermissions(OperatorId).ToList();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region CRUD Operations

        //Create new OperatorPermission.
        public void Create(List<OperatorPermission> lstOperatorPermission)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    Delete(lstOperatorPermission[0].OperatorId);
                    foreach (OperatorPermission p in lstOperatorPermission)
                    {
                        p.CreatedOn = DateTime.Now;
                        ctx.OperatorPermissions.Add(p);
                    }
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete OperatorPermission record by OperatorId.
        public void Delete(int OperatorId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var lstOperatorPermissions = ctx.OperatorPermissions.Where(p => p.OperatorId == OperatorId).ToList();
                    foreach (var obj in lstOperatorPermissions)
                    {
                        ctx.OperatorPermissions.Remove(obj);
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
