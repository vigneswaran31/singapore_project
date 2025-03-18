using HelpDeskEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpDeskEntity.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace HelpDeskBAL
{
    public class UserBL
    {
        #region Get Methods

        //Get User details by Name
        public User GetByUserName(string UserName)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Users.Where(p => p.Email == UserName).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get User data list in Grid Format by UserId.
        public string GetGridData(GridSettings grid, bool IsCompanyUsers, int UserId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    IQueryable<vw_User> query;

                    if (IsCompanyUsers)
                        query = ctx.vw_User.Where(p => p.CompanyId != null).AsQueryable();
                    else
                        query = ctx.vw_User.Where(p => p.CompanyId == null).AsQueryable();

                    if (UserId > 0)
                        query = query.Where(p => p.UserId != UserId);

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
                                    UserId = c.UserId,
                                    Name = c.Name,
                                    Email = c.Email,
                                    Contact = c.Contact,
                                    IsEnable = c.IsEnable,
                                    RoleName = c.RoleName,
                                    CreatedOn = c.CreatedOn,
                                    CreatedBy = c.CreatedBy,
                                    Action = c.UserId
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

        //Get User data list in Grid Format by CompanyId.
        public string GetGridDataByCompanyId(GridSettings grid, int CompanyId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    IQueryable<vw_User> query;

                    query = ctx.vw_User.Where(p => p.CompanyId == CompanyId).AsQueryable();

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
                                    UserId = c.UserId,
                                    Name = c.Name,
                                    Email = c.Email,
                                    Contact = c.Contact,
                                    IsEnable = c.IsEnable,
                                    IsSuperUser = c.RoleName == En_CompanyUserRole.CompanySuperUser.ToString() ? true : false,
                                    CreatedOn = c.CreatedOn,
                                    CreatedBy = c.CreatedBy,
                                    Action = c.UserId
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

        //Get User details by UserId
        public User GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Users.Where(u => u.UserId == id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<SelectListItem> GetAllByRole(string Role)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    var lstUsers = ctx.vw_User.Where(p => p.RoleName == Role && p.IsEnable == true).OrderBy(p => p.Name).ToList();
                    List<SelectListItem> lst = new List<SelectListItem>();

                    foreach (var obj in lstUsers)
                    {
                        lst.Add(new SelectListItem
                        {
                            Text = obj.Name + " " + "(" + obj.Email + ")",
                            Value = obj.UserId.ToString(),
                        });
                    }
                    return lst;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get all Users by CompanyId
        public List<User> GetUsersByCompanyId(int CompanyId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Users.Where(u => u.CompanyId == CompanyId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get all Company users list.
        public List<vw_CompanyUser> GetAllCompanyUser()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.vw_CompanyUser.Where(t => t.IsEnable == true).OrderBy(u => u.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get All CompanyUsers list records ByCompanyId
        public List<vw_CompanyUser> GetAllCompanyUserByCompanyId(int CompanyId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.vw_CompanyUser.Where(c => c.CompanyId == CompanyId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get all counts for Dashboard.
        public DashboardCountModel GetAllCounts()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    DashboardCountModel oDasboardCountModel = new DashboardCountModel();
                    var cmd = ctx.Database.Connection.CreateCommand();
                    cmd.CommandText = "[dbo].[sp_GetAllcounts]";
                    try
                    {
                        ctx.Database.Connection.Open();
                        var reader = cmd.ExecuteReader(); // Run the sproc  

                        reader.Read();
                        oDasboardCountModel.SupportTickets = int.Parse(reader["SupportTickets"].ToString());

                        reader.NextResult();
                        reader.Read();
                        oDasboardCountModel.SupportCategories = int.Parse(reader["SupportCategories"].ToString());// move to next result set 

                        reader.NextResult();
                        reader.Read();
                        oDasboardCountModel.Admins = int.Parse(reader["Admins"].ToString());

                        reader.NextResult();
                        reader.Read();
                        oDasboardCountModel.Operators = int.Parse(reader["Operators"].ToString());// move to next result set 

                        reader.NextResult();
                        reader.Read();
                        oDasboardCountModel.SupportTeams = int.Parse(reader["SupportTeams"].ToString());// move to next result set 

                        reader.NextResult();
                        reader.Read();
                        oDasboardCountModel.Companies = int.Parse(reader["Companies"].ToString());// move to next result set 

                        reader.NextResult();
                        reader.Read();
                        oDasboardCountModel.KBCategories = int.Parse(reader["KBCategories"].ToString());// move to next result set 

                        reader.NextResult();
                        reader.Read();
                        oDasboardCountModel.KBArticles = int.Parse(reader["KBArticles"].ToString());// move to next result set 

                        reader.NextResult();
                        reader.Read();
                        oDasboardCountModel.EmailInbox = int.Parse(reader["EmailInbox"].ToString());// move to next result set 

                        return oDasboardCountModel;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        ctx.Database.Connection.Close();
                    }
                    //return ctx.sp_GetAllcounts().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Check Validation

        //Check for Validate a User.
        public bool ValidateUser(string login, string password)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    password = Utility.CommonFunction.GetSHAHash(password, login);

                    User oUser = ctx.Users.Where(p => p.Email == login && p.Password == password && p.IsEnable == true).FirstOrDefault();

                    if (oUser != null)
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

        //Create new User record.
        public void Create(User user)
        {
            try
            {
                if (user.RoleId == (int)En_CompanyUserRole.CompanySuperUser)
                {
                    ResetDefaultCompanyUser(user.CompanyId.Value);
                }
                using (var ctx = new HelpDeskEntities())
                {
                    user.Password = Utility.CommonFunction.GetSHAHash(user.Password, user.Email);
                    user.CreatedBy = Utility.CommonFunction.GetLoginUserName();
                    user.CreatedOn = DateTime.Now;
                    user.ModifiedBy = Utility.CommonFunction.GetLoginUserName();
                    user.ModifiedOn = DateTime.Now;
                    ctx.Users.Add(user);
                    ctx.SaveChanges();
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update Existing User record.
        public void Update(User oUser)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    if (oUser.RoleId == (int)En_CompanyUserRole.CompanySuperUser)
                    {
                        ResetDefaultCompanyUser(oUser.CompanyId.Value);
                    }

                    User oldUser = GetById(oUser.UserId);

                    oldUser.Password = Utility.CommonFunction.GetSHAHash(oUser.Password == null ? oldUser.Password : oUser.Password, oUser.Email);
                    oldUser.Email = oUser.Email;
                    oldUser.Name = oUser.Name;
                    oldUser.Contact = oUser.Contact;
                    oldUser.IsEnable = oUser.IsEnable;
                    oldUser.RoleId = oUser.RoleId;
                    oldUser.VerificationToken = oUser.VerificationToken;
                    oldUser.VerificationTokenExpirationDate = oUser.VerificationTokenExpirationDate;
                    oldUser.ModifiedBy = Utility.CommonFunction.GetLoginUserName();
                    oldUser.ModifiedOn = DateTime.Now;

                    ctx.Entry(oldUser).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete User record by Id
        public bool Delete(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    User oUser = ctx.Users.Where(p => p.UserId == id).FirstOrDefault();
                    ctx.Users.Remove(oUser);
                    ctx.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Update User Profile record
        public User UpdateProfile(User ouser)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    User objUser = ctx.Users.Where(u => u.UserId == ouser.UserId).FirstOrDefault();
                    objUser.Name = ouser.Name;
                    objUser.Contact = ouser.Contact;

                    ctx.Entry(objUser).State = EntityState.Modified;
                    ctx.SaveChanges();

                    return objUser;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public void ResetDefaultCompanyUser(int CompanyId)
        {
            using (var ctx = new HelpDeskEntities())
            {
                List<User> lstCompanyUsers = ctx.Users.Where(u => u.CompanyId == CompanyId).ToList();
                if (lstCompanyUsers.Count > 0 && lstCompanyUsers != null)
                {
                    lstCompanyUsers.ForEach(a => a.RoleId = Convert.ToInt16(En_CompanyUserRole.CompanyUser));
                    ctx.SaveChanges();
                }

            }
        }
        public void ChangePassword(string password, User oUser)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    ctx.Database.ExecuteSqlCommand("update Users set password='" + Utility.CommonFunction.GetSHAHash(password, oUser.Email) + "' where UserId='" + oUser.UserId + "'");
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CheckEmail(string email, int userId)
        {
            string strEmail = null;
            using (var ctx = new HelpDeskEntities())
            {
                var varUser = ctx.Users.Where(p => p.Email == email && p.UserId != userId).FirstOrDefault();

                if (varUser == null)
                    return null;
                else
                {
                    if (varUser != null)
                    {
                        strEmail = varUser.Email;
                    }
                    return strEmail;
                }
            }
        }
    }
}
