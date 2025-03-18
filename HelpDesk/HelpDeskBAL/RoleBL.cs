using HelpDeskEntity;
using HelpDeskEntity.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HelpDeskBAL
{
    public class RoleBL
    {
        #region Get Methods

        public string GetRoleFilter()
        {
            try
            {
                List<SelectListItem> lstRoles = new List<SelectListItem>();

                var Roles = new[]{
                    new SelectListItem{Text = En_Role.Admin.ToString(), Value =En_Role.Admin.ToString()},
                    new SelectListItem{Text = En_Role.Operator.ToString(), Value = En_Role.Operator.ToString()},
                   };

                string Role = ":All;";

                 lstRoles = Roles.ToList();

                for (int i = 0; i < lstRoles.Count; i++)
                    Role = Role + lstRoles[i].Value + ":" + lstRoles[i].Value + ";";

                Role = Role.Remove(Role.Length - 1);
                return Role;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetCompanyUserRoleFilter()
        {
            try
            {
                List<SelectListItem> lstRoles = new List<SelectListItem>();

                var Roles = new[]{
                    new SelectListItem{Text = En_CompanyUserRole.CompanyUser.ToString(), Value =En_CompanyUserRole.CompanyUser.ToString()},
                    new SelectListItem{Text = En_CompanyUserRole.CompanySuperUser.ToString(), Value = En_CompanyUserRole.CompanySuperUser.ToString()},
                    };

                string Role = ":All;";

                lstRoles = Roles.ToList();

                for (int i = 0; i < lstRoles.Count; i++)
                    Role = Role + lstRoles[i].Value + ":" + lstRoles[i].Value + ";";

                Role = Role.Remove(Role.Length - 1);
                return Role;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Role> GetAll()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Roles.ToList();
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
