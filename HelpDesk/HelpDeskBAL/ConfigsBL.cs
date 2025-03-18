using HelpDeskEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskBAL
{
    public class ConfigsBL
    {
        #region Get Methods

        //Get all Config List records.
        public List<Config> GetAll()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Configs.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Config record by Name.
        public Config GetByName(string Name)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Configs.Where(p => p.Name == Name).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Operations

        //Update Existing  Configs.
        public void Update(IList<Config> lstConfig)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    foreach (var item in lstConfig)
                    {
                        var obj = ctx.Configs.Where(c => c.Id == item.Id).FirstOrDefault();
                        obj.Value = item.Value;
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
