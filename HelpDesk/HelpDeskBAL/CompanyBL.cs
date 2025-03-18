using HelpDeskEntity;
using HelpDeskEntity.Classes;
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
    public class CompanyBL
    {
        #region Get Methods

        //Get Company detais record by CompanyId.
        public Company GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Companies.Where(c => c.CompanyId == id).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get Companies detail list in Grid Format.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.Companies.Include("Company1").AsQueryable();


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
                                    CompanyID = c.CompanyId,
                                    CategoryId=c.CategoryId,
                                    Name = c.ParentId !=  null ? c.CompanyName + " ( " + c.Company1.CompanyName + " )" : c.CompanyName, 
                                    Address = c.Address,
                                    CreatedOn = c.CreatedOn,
                                    CreatedBy = c.CreatedBy,
                                    ModifiedOn = c.ModifiedOn,
                                    ModifiedBy = c.ModifiedBy,
                                    IsEnable=c.IsEnable,
                                    Note=c.Note,
                                    CAP=c.CAP,
                                    City=c.City,
                                    Contact=c.Contact,
                                    VAT=c.VAT,
                                    Mail=c.Mail,
                                    PEC=c.PEC,
                                    BadgeFormat=c.BadgeFormat,
                                    AccessId=c.AccessId,
                                    UseSalePriceNr=c.UseSalePriceNr,
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

        //Get all Enable Companies detail list
        public List<Company> GetAllEnableCompanies()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Companies.Where(c => c.IsEnable == true).OrderBy(p => p.CompanyName).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Get all parents
        public List<Company> GetAllParents()
        {
            try
            {
                using(var ctx = new HelpDeskEntities())
                {
                    return ctx.Companies.Where(p => p.ParentId == null && p.IsEnable == true).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Operations

        //Create new Company record.
        public void Create(Company oCompany)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oCompany.CreatedBy = HttpContext.Current.User.Identity.Name;
                    oCompany.CreatedOn = DateTime.Now;
                    oCompany.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oCompany.ModifiedOn = DateTime.Now;
                    ctx.Companies.Add(oCompany);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update Existing Company record.
        public void Update(Company oCompany)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oCompany.ModifiedBy = HttpContext.Current.User.Identity.Name;
                    oCompany.ModifiedOn = DateTime.Now;
                    ctx.Entry(oCompany).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete Company record by Id
        public void Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    new AttachmentsBL().DeleteByLinkId(id, Constants.CompanyImgUploadPath.ToString());
                    Company oCompany = ctx.Companies.Where(c => c.CompanyId == id).FirstOrDefault();
                    ctx.Companies.Remove(oCompany);
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
