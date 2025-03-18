using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpDeskEntity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data;
using System.Web;

namespace HelpDeskBAL
{
    public class ContractTemplateBL
    {
        #region Get Methods

        //Get Contract Template record detail by id
        public ContractTemplate GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.ContractTemplates.Where(c => c.ContractTemplateId == id).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get Contract Template data list in Grid Format.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.ContractTemplates.AsQueryable();


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
                                    ContractTemplateId = c.ContractTemplateId,
                                    TemplateName = c.TemplateName,
                                    Description = c.Description,
                                    NoOfTickets = c.NoOfTickets,
                                    ResponseWithInHours = c.ResponseWithInHours,
                                    SolutionWithInHours = c.SolutionWithInHours,
                                    Hours = c.Hours,
                                    IsEnable = c.IsEnable,
                                    CreatedOn = c.CreatedOn,
                                    CreatedBy = c.CreatedBy,
                                    ModifiedOn = c.ModifiedOn,
                                    ModifiedBy = c.ModifiedBy,
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

        //Get all Enable Contract Template list records
        public List<ContractTemplate> GetAllEnableContractTemplates()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.ContractTemplates.Where(c => c.IsEnable == true).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Check to If SLA Allowed on Contract by ContractId
        public bool IsSlaAllowOnContractById(int ContractId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    vw_CompanyContract oContractTemplate = ctx.vw_CompanyContract.Where(c => c.CompanyContractId == ContractId).FirstOrDefault();

                    if (oContractTemplate != null)
                    {
                        if (oContractTemplate.ResponseWithInHours != null && oContractTemplate.SolutionWithInHours != null)
                            return true;
                        else
                            return false;
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

        #region CRUD Operations

        //Create new ContractTemplate record.
        public void Create(ContractTemplate oContractTemplate)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {

                    oContractTemplate.CreatedBy = HttpContext.Current.User.Identity.Name;
                    oContractTemplate.CreatedOn = DateTime.Now;
                    oContractTemplate.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oContractTemplate.ModifiedOn = DateTime.Now;
                    ctx.ContractTemplates.Add(oContractTemplate);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update Existing ContractTemplate record.
        public void Update(ContractTemplate oContractTemplate)
        {
            try
            {

                using (var ctx = new HelpDeskEntities())
                {
                    oContractTemplate.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oContractTemplate.ModifiedOn = DateTime.Now;
                    ctx.Entry(oContractTemplate).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete ContractTemplate record by Id
        public void Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    ContractTemplate oContractTemplate = ctx.ContractTemplates.Where(c => c.ContractTemplateId == id).FirstOrDefault();
                    ctx.ContractTemplates.Remove(oContractTemplate);
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
