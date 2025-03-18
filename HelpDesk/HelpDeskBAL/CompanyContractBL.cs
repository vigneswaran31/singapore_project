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
    public class CompanyContractBL
    {
        #region Get Methods

        //Get CompanyContract record detail by id
        public CompanyContract GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.CompanyContracts.Where(c => c.CompanyContractId == id).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get CompanyContract view record detail by id
        public vw_CompanyContract GetByCompanyContractId(int CompanyContractId)
        {

            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.vw_CompanyContract.Where(c => c.CompanyContractId == CompanyContractId).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get CompanyContract data list in Grid Format.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.vw_CompanyContract.AsQueryable();


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
                                    CompanyContractId = c.CompanyContractId,
                                    ContractNumber = c.ContractNumber,
                                    CompanyId = c.CompanyId,
                                    CompanyName = c.CompanyName,
                                    ContractTemplateId = c.ContractTemplateId,
                                    TemplateName = c.TemplateName,
                                    StartDate = c.StartDate,
                                    EndDate = c.EndDate,
                                    Price = c.Price,
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

        //Get CompanyContract data list by company Id in Grid Format.
        public string GetGridDataByCompanyId(GridSettings grid, int CompanyId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.vw_CompanyContract.Where(p => p.CompanyId == CompanyId).AsQueryable();


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
                                    CompanyContractId = c.CompanyContractId,
                                    ContractNumber = c.ContractNumber,
                                    CompanyId = c.CompanyId,
                                    CompanyName = c.CompanyName,
                                    ContractTemplateId = c.ContractTemplateId,
                                    TemplateName = c.TemplateName,
                                    StartDate = c.StartDate,
                                    EndDate = c.EndDate,
                                    Price = c.Price,
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


        //Get Company's Active Contact Details by CompanyId
        public vw_CompanyContract GetActiveContractDetailByCompanyId(int CompanyId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    vw_CompanyContract oCompanyContract = ctx.vw_CompanyContract.Where(c => c.CompanyId == CompanyId && c.StartDate <= DateTime.Today && c.EndDate >= DateTime.Today).FirstOrDefault();
                    if (oCompanyContract == null)
                    {
                        Company company = new CompanyBL().GetById(CompanyId);
                        if (company.ParentId != null && company.ShareParentContract == true)
                        {
                            oCompanyContract = ctx.vw_CompanyContract.Where(c => c.CompanyId == company.ParentId && c.StartDate <= DateTime.Today && c.EndDate >= DateTime.Today).FirstOrDefault();
                        }
                    }
                    return oCompanyContract;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Nullable<int> GetContractRemainingTime(int CompanyContractId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                   // return ctx.GetContractRemainingTime(CompanyContractId).FirstOrDefault().Value;

                    var RemainingTime = ctx.GetContractRemainingTime(CompanyContractId).FirstOrDefault();
                   
                    if (RemainingTime == null)
                        return null;
                    else
                        return RemainingTime.Value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Contact Details by CompanyId and ContractId
        public vw_CompanyContract GetCompanyContractDetail(int CompanyId, int ContractId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.vw_CompanyContract.Where(c => c.CompanyContractId == ContractId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //Get ContractIds of Active Contract of all Companies. //this query for  Statistics at Admin side.
        public List<int> GetActiveContractCompanies_ContractIds()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.vw_CompanyContract.Where(c => c.IsEnable == true && c.StartDate <= DateTime.Today && c.EndDate >= DateTime.Today).Select(c => c.CompanyContractId).ToList();
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public List<vw_CompanyContract> GetAllCompanyContracts()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.vw_CompanyContract.ToList();
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public bool IsContractActive(int ContractId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    vw_CompanyContract oData = ctx.vw_CompanyContract.Where(c => c.CompanyContractId == ContractId && c.StartDate <= DateTime.Today && c.EndDate >= DateTime.Today).FirstOrDefault();
                    if (oData != null)
                        return true;
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

        //Create new CompanyContract record.
        public void Create(CompanyContract oCompanyContract)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oCompanyContract.CreatedBy = HttpContext.Current.User.Identity.Name;
                    oCompanyContract.CreatedOn = DateTime.Now;
                    oCompanyContract.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oCompanyContract.ModifiedOn = DateTime.Now;
                    ctx.CompanyContracts.Add(oCompanyContract);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update Existing CompanyContract record.
        public void Update(CompanyContract oCompanyContract)
        {
            try
            {

                using (var ctx = new HelpDeskEntities())
                {

                    oCompanyContract.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oCompanyContract.ModifiedOn = DateTime.Now;
                    ctx.Entry(oCompanyContract).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete CompanyContract record by Id
        public void Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    CompanyContract oCompanyContract = ctx.CompanyContracts.Where(c => c.CompanyContractId == id).FirstOrDefault();
                    ctx.CompanyContracts.Remove(oCompanyContract);
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
