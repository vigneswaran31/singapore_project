using HelpDeskEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskBAL
{
    public class PermissionBL
    {
        #region Get Methods

        //Get All Permissions record
        public List<Permission> GetAllPermission()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Permissions.ToList();
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
