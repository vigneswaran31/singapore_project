using HelpDeskEntity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpDeskEntity.Classes;
using HelpDeskEntity.Models;
using System.Transactions;
namespace HelpDeskBAL
{
    public class SupportTicketBL
    {
        #region Get Methods

        //Get Support Ticket Details by Id.
        public Ticket GetById(int id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Tickets.Where(c => c.TicketId == id).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Ticket GetByTicketViewId(string id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Tickets.Where(c => c.TicketViewId == id).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get Support Ticket Details by Guid.
        public Ticket GetByTicketId(Guid id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.Tickets.Where(c => c.TicketGuid == id).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get Support Ticket data list in Grid Format for Operator and Admin.
        public string GetAllTickets(GridSettings grid, int OperatorId = 0, SearchTicketModel oSearchTicketModel = null)
        {
            try
            {
                string ResponseStatusData = string.Empty;
                string SolutionStatusDate = string.Empty;
                using (var ctx = new HelpDeskEntities())
                {
                    var query = ctx.vw_SupportTicket.AsQueryable();

                    if (oSearchTicketModel == null) //if advance search not available show only not closed tickets
                    {
                        List<TicketStatu> lstTicketStatu = new TicketStatusBL().GetAllOpenStatus();
                        List<int> lstStatusId = lstTicketStatu.Select(a => a.TicketStatusId).ToList();

                        query = query.Where(t => lstStatusId.Contains(t.CurrentStatusId)).AsQueryable();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(oSearchTicketModel.ProblemDescription))
                            query = query.Where(t => t.Title.Contains(oSearchTicketModel.ProblemDescription) || t.Title.Contains(oSearchTicketModel.ProblemDescription));

                        if (oSearchTicketModel.Status != 0)
                            query = query.Where(t => t.CurrentStatusId == oSearchTicketModel.Status);

                        if (oSearchTicketModel.Company != 0)
                            query = query.Where(t => t.CompanyId == oSearchTicketModel.Company);

                        if (oSearchTicketModel.OperatorPriority != 0)
                            query = query.Where(t => t.OperatorPriorityId == oSearchTicketModel.OperatorPriority);

                        if (oSearchTicketModel.CustomerPriority != 0)
                            query = query.Where(t => t.CustomerPriorityId == oSearchTicketModel.CustomerPriority);

                        if (oSearchTicketModel.AssignedToOperator != 0)
                            query = query.Where(t => t.AssignToOperator == oSearchTicketModel.AssignedToOperator);

                        if (oSearchTicketModel.AssignedToSupportTeam != 0)
                            query = query.Where(t => t.AssignToSupportTeam == oSearchTicketModel.AssignedToSupportTeam);

                        if (oSearchTicketModel.Category != 0)
                            query = query.Where(t => t.CategoryId == oSearchTicketModel.Category);

                        if (oSearchTicketModel.DateTicketFrom != null)
                            query = query.Where(o => o.CreatedOn >= oSearchTicketModel.DateTicketFrom);

                        if (oSearchTicketModel.DateTicketTo != null)
                            query = query.Where(o => o.CreatedOn <= oSearchTicketModel.DateTicketTo);

                    }

                    if (OperatorId > 0)
                    {
                        if (!new CommonBL().IsPermissionAllow(OperatorId, ((int)En_Permission.ViewOthersTickets)))
                        {
                            List<int> lstTeamId = new SupportTeamMembersBL().GetAllTeamByMemberId(OperatorId).Select(t => t.TeamId).ToList();
                            query = query.Where(t => t.AssignToOperator == OperatorId || lstTeamId.Contains(t.AssignToSupportTeam.Value));
                        }
                    }

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
                                    TicketId = c.TicketId,
                                    TicketGuid = c.TicketGuid,
                                    TicketViewId = c.TicketViewId,
                                    Title = c.Title,
                                    CompanyName = c.CompanyName,
                                    CategoryName = c.CategoryName,
                                    StatusName = c.StatusName,
                                    StatusColor = c.StatusColor,
                                    OperatorPriority = c.OperatorPriority,
                                    CustomerPriority = c.CustomerPriority,
                                    CompanyUserName = c.CompanyUserName,
                                    AssignToName = c.AssignToName,
                                    ClosedOn = c.ClosedOn,
                                    ClosedBy = c.ClosedBy,
                                    ResponseDeadline = c.ResponseDeadline,
                                    SolutionDeadline = c.SolutionDeadline,
                                    ResponseDate = c.ResponseDate,
                                    SolutionDate = c.SolutionDate,
                                    ResponseStatus = c.ResponseStatus,
                                    SolutionStatus = c.SolutionStatus,
                                    ResponseStatusData = c.ResponseDeadline != null ? SLAResponseFormatter(c.ResponseStatus, c.ResponseDeadline.Value, c.SolutionInPercentage, c.ResponseDate) : "N/A",
                                    SolutionStatusDate = c.SolutionDeadline != null ? SLASolutionFormatter(c.SolutionStatus, c.SolutionDeadline.Value, c.SolutionInPercentage, c.ResponseStatus, c.SolutionDate) : "N/A",
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

        //Get Support Ticket data list in Grid Format for User.
        public string GetAllUserTickets(GridSettings grid, int UserId = 0, UserSearchTicketModel oSearchTicketModel = null)
        {
            try
            {
                string ResponseStatusData = string.Empty;
                string SolutionStatusDate = string.Empty;
                User oUser=null;
                using (var ctx = new HelpDeskEntities())
                {
                    oUser = ctx.Users.Where(u => u.UserId == UserId).FirstOrDefault();
                    var query = ctx.vw_SupportTicket.Where(t=>t.CompanyId==oUser.CompanyId).AsQueryable();
                    
                    if (oSearchTicketModel == null) //if advance search not available show only not closed tickets
                    {
                        List<int> lstStatusId = new TicketStatusBL().GetAllOpenStatus().Select(a => a.TicketStatusId).ToList();
                        query = query.Where(t => lstStatusId.Contains(t.CurrentStatusId)).AsQueryable();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(oSearchTicketModel.ProblemDescription))
                            query = query.Where(t => t.Title.Contains(oSearchTicketModel.ProblemDescription) || t.Title.Contains(oSearchTicketModel.ProblemDescription));

                        if (oSearchTicketModel.Status != 0)
                            query = query.Where(t => t.CurrentStatusId == oSearchTicketModel.Status);


                        if (oSearchTicketModel.CustomerPriority != 0)
                            query = query.Where(t => t.CustomerPriorityId == oSearchTicketModel.CustomerPriority);


                        if (oSearchTicketModel.Category != 0)
                            query = query.Where(t => t.CategoryId == oSearchTicketModel.Category);

                        if (oSearchTicketModel.CompanyUser > 0)
                            query = query.Where(t => t.CompanyUserId == oSearchTicketModel.CompanyUser);

                        if (oSearchTicketModel.DateTicketFrom != null)
                            query = query.Where(o => o.CreatedOn >= oSearchTicketModel.DateTicketFrom);

                        if (oSearchTicketModel.DateTicketTo != null)
                            query = query.Where(o => o.CreatedOn <= oSearchTicketModel.DateTicketTo);

                    }
                    if (oUser.RoleId!=(int)En_CompanyUserRole.CompanySuperUser)
                    {
                        query = query.Where(t => t.CompanyUserId == oUser.UserId);
                    }

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
                                    TicketId = c.TicketId,
                                    TicketGuid = c.TicketGuid,
                                    TicketViewId=c.TicketViewId,                                    
                                    Title = c.Title,
                                    CompanyName = c.CompanyName,
                                    CategoryName = c.CategoryName,
                                    StatusName = c.StatusName,
                                    StatusColor = c.StatusColor,
                                    CustomerPriority = c.CustomerPriority,
                                    CompanyUserName = c.CompanyUserName,
                                    AssignToName = c.AssignToName,
                                    ClosedOn = c.ClosedOn,
                                    ClosedBy = c.ClosedBy,
                                    ResponseDeadline = c.ResponseDeadline,
                                    SolutionDeadline = c.SolutionDeadline,
                                    ResponseDate = c.ResponseDate,
                                    SolutionDate = c.SolutionDate,
                                    ResponseStatus = c.ResponseStatus,
                                    SolutionStatus = c.SolutionStatus,
                                    ResponseStatusData = c.ResponseDeadline != null ? SLAResponseFormatter(c.ResponseStatus, c.ResponseDeadline.Value, c.SolutionInPercentage, c.ResponseDate) : "N/A",
                                    SolutionStatusDate = c.SolutionDeadline != null ? SLASolutionFormatter(c.SolutionStatus, c.SolutionDeadline.Value, c.SolutionInPercentage, c.ResponseStatus, c.SolutionDate) : "N/A",
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

        //Get Support Ticket View Details by TicketId.
        public vw_SupportTicket GetTicketViewDetailById(int TicketId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.vw_SupportTicket.Where(c => c.TicketId == TicketId).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<vw_SupportTicket> GetAllOpenTickets()
        {
            try
            {
                using(var ctx = new HelpDeskEntities())
                {
                    List<TicketStatu> lstTicketStatu = new TicketStatusBL().GetAllOpenStatus();
                    List<int> lstStatusId = lstTicketStatu.Select(a => a.TicketStatusId).ToList();

                    return ctx.vw_SupportTicket.Where(t => lstStatusId.Contains(t.CurrentStatusId)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SLAResponseFormatter(string ResponseStatus, DateTime ResponseDeadline, string SLAPercentage, DateTime? ResponseDate)
        {
            if (ResponseStatus == "ResPending")
            {
                return "(" + SLAPercentage + "%) <a href\"javascript:void(0)\" title=\"Response Due on " + ResponseDeadline.ToString() + "\"><img src=\"../../Content/images/r-icon.png\" /></a>";
            }
            else if (ResponseStatus == "ResOutofSLA")
            {
                return "<a href\"javascript:void(0)\" title=\"Ticket is Out of SLA\"><img src=\"../../Content/images/r2-icon.gif\" /></a>";
            }
            else if (ResponseStatus == "ResDone")
            {
                return "<a href\"javascript:void(0)\" title=\"Responded on " + ResponseDate.ToString() + " \"><img src=\"../../Content/images/r1-icon.png\" /></a>";
            }
            else
            {
                return ResponseStatus;
            }
        }

        public string SLASolutionFormatter(string SolutionStatus, DateTime SolutionDeadline, string SLAPercentage, string ResponseStatus, DateTime? SolutionDate)
        {
            if (SolutionStatus == "SolPending")
            {
                string strReturn = string.Empty;
                if (ResponseStatus != "ResPending")
                {
                    strReturn += "(" + SLAPercentage + "%) ";
                }
                return strReturn += "<a href\"javascript:void(0)\" title=\"Solution Due on " + SolutionDeadline.ToString() + "\"><img src=\"../../Content/images/s-icon.png\" /></a>";
            }
            else if (SolutionStatus == "SolOutofSLA")
            {
                return "<a href\"javascript:void(0)\" title=\"Ticket is Out of SLA\"><img src=\"../../Content/images/s2-icon.gif\" /></a>";
            }
            else if (SolutionStatus == "SolDone")
            {
                return "<a href\"javascript:void(0)\" title=\"Solution on " + SolutionDate.ToString() + "\"><img src=\"../../Content/images/s1-icon.png\" /></a>";
            }
            else
            {
                return SolutionStatus;
            }
        }

        //Get Support Ticket View Details by Guid.
        public vw_SupportTicket GetTicketViewDetailByGuid(Guid TicketId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.vw_SupportTicket.Where(c => c.TicketGuid == TicketId).FirstOrDefault();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get Dashboard count for Operator side.
        public OperatorDashboardCount GetOperatorDashboardCount(int UserId)
        {
            try
            {
                OperatorDashboardCount oDashboardCount = new OperatorDashboardCount();
                using (var ctx = new HelpDeskEntities())
                {
                    List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
                    var a = ctx.vw_SupportTicket.Where(t=>lstOpenStatusId.Contains(t.CurrentStatusId)).Select(t =>  new { t.TicketId, t.SolutionInPercentage, t.AssignToOperator,  t.AssignToSupportTeam}).ToList();

                    if (a != null)
                    {
                        oDashboardCount.AssignedToMe_Below_fifty = a.Where(t => t.AssignToOperator == UserId && int.Parse(t.SolutionInPercentage) <50).Count();
                        oDashboardCount.AssignedToMe_Beetween_Fifty_Eighty = a.Where(t => t.AssignToOperator == UserId && int.Parse(t.SolutionInPercentage) >= 50 && int.Parse(t.SolutionInPercentage) <= 80).Count();
                        oDashboardCount.AssignedToMe_Above_Eighty = a.Where(t => t.AssignToOperator == UserId && int.Parse(t.SolutionInPercentage) > 80).Count();

                        if (new CommonBL().IsPermissionAllow(UserId, ((int)En_Permission.ViewOthersTickets)))
                        {
                            oDashboardCount.AssignedToTeams_Below_fifty = a.Where(t => t.AssignToSupportTeam != null && int.Parse(t.SolutionInPercentage) < 50).Count();
                            oDashboardCount.AssignedToTeams_Beetween_Fifty_Eighty = a.Where(t => t.AssignToSupportTeam != null && int.Parse(t.SolutionInPercentage) >= 50 && int.Parse(t.SolutionInPercentage) <= 80).Count();
                            oDashboardCount.AssignedToTeams_Above_Eighty = a.Where(t => t.AssignToSupportTeam != null && int.Parse(t.SolutionInPercentage) > 80).Count();
                        }
                        else
                        {
                            List<int> lstTeamId = new SupportTeamMembersBL().GetAllTeamByMemberId(UserId).Select(t => t.TeamId).ToList();
                            oDashboardCount.AssignedToTeams_Below_fifty = a.Where(t => lstTeamId.Contains(t.AssignToSupportTeam!=null?t.AssignToSupportTeam.Value:0) && int.Parse(t.SolutionInPercentage) < 50).Count();
                            oDashboardCount.AssignedToTeams_Beetween_Fifty_Eighty = a.Where(t => lstTeamId.Contains(t.AssignToSupportTeam != null ? t.AssignToSupportTeam.Value : 0) && int.Parse(t.SolutionInPercentage) >= 50 && int.Parse(t.SolutionInPercentage) <= 80).Count();
                            oDashboardCount.AssignedToTeams_Above_Eighty = a.Where(t => lstTeamId.Contains(t.AssignToSupportTeam != null ? t.AssignToSupportTeam.Value : 0) && int.Parse(t.SolutionInPercentage) > 80).Count();
                        }

                        if (new CommonBL().IsPermissionAllow(UserId, ((int)En_Permission.ViewOthersTickets)))
                        {
                            oDashboardCount.UnassignedTickets_Below_fifty = a.Where(t => t.AssignToSupportTeam == null && t.AssignToOperator == null && int.Parse(t.SolutionInPercentage) < 50).Count();
                            oDashboardCount.UnassignedTickets_Beetween_Fifty_Eighty = a.Where(t => t.AssignToSupportTeam == null && t.AssignToOperator == null && int.Parse(t.SolutionInPercentage) >= 50 && int.Parse(t.SolutionInPercentage) <= 80).Count();
                            oDashboardCount.UnassignedTickets_Above_Eighty = a.Where(t => t.AssignToSupportTeam == null && t.AssignToOperator == null && int.Parse(t.SolutionInPercentage) > 80).Count();
                        }
                    }
                }
                return oDashboardCount;

            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get all Suppoet Ticket view list record
        public List<vw_SupportTicket> GetAllTickets()
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.vw_SupportTicket.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Get Dashboard count for User side.
        public int GetUserDashboardCount(User oUser)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    if (oUser.RoleId==(int)En_CompanyUserRole.CompanySuperUser)
	                {
                        List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
                        return ctx.Tickets.Where(t => t.CompanyId == oUser.CompanyId.Value && lstOpenStatusId.Contains(t.CurrentStatus)).Count();
	                }
                 else
	                {
                        List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
                        return ctx.Tickets.Where(t => t.CompanyUserId == oUser.UserId && lstOpenStatusId.Contains(t.CurrentStatus)).Count();
	                }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //Check for if Support Ticket's Status is closed or not.
        public bool IsSetCloseStatus(Ticket oTicket)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    bool IsClosed = ctx.TicketStatus.Where(s => s.TicketStatusId == oTicket.CurrentStatus).Select(s => s.IsClosedStatus).FirstOrDefault();
                    return IsClosed;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region CRUD Operations

        //Create new Support Ticket record.
        public Ticket Create(Ticket oTicket)
        {
            vw_CompanyContract oContractTemplate = new CompanyContractBL().GetCompanyContractDetail(oTicket.CompanyId,oTicket.ContractId);
            if (oContractTemplate.ResponseWithInHours != null && oContractTemplate.SolutionWithInHours != null)
            {
                oTicket.ResponseDeadline = DateTime.Now.AddHours(Convert.ToDouble(oContractTemplate.ResponseWithInHours));
                oTicket.SolutionDeadline = DateTime.Now.AddHours(Convert.ToDouble(oContractTemplate.SolutionWithInHours));
            }
            using (var scope = new TransactionScope())
            {
                try
                {
                    using (var ctx = new HelpDeskEntities())
                    {
                        if (oTicket.AssignToOperator == null && oTicket.AssignToSupportTeam == null)
                        {
                            int OperatorId = new AutoAssignBL().AutoAssignOperator(oTicket.CompanyId, oTicket.CategoryId);
                            if (OperatorId > 0)
                                oTicket.AssignToOperator = OperatorId;
                            else
                            {
                                int SupportTeamId = new AutoAssignBL().AutoAssignSupportTeam(oTicket.CompanyId, oTicket.CategoryId);
                                if (SupportTeamId > 0)
                                    oTicket.AssignToSupportTeam = SupportTeamId;
                            }
                        }

                        Config oCongig= new ConfigsBL().GetByName(EntityNames.TicketViewNumber.ToString());
                        string TicketViewNo = oCongig.Value;
                        int NextTicketNumber = Convert.ToInt32(TicketViewNo) + 1;
                        oCongig.Value = NextTicketNumber.ToString();
                        ctx.Entry(oCongig).State = EntityState.Modified;
                        ctx.SaveChanges();

                        oTicket.TicketViewId = GetNextTicketNumber(TicketViewNo);
                        oTicket.CreatedOn = DateTime.Now;
                        ctx.Tickets.Add(oTicket);
                        ctx.SaveChanges();

                        scope.Complete();
                        return oTicket;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        //Update Existing Support Ticket record.
        public Ticket Update(Ticket oTicket, int UserId)
        {
            try
            {
               

                using (var ctx = new HelpDeskEntities())
                {
                    Ticket ObjTicket = new SupportTicketBL().GetByTicketId(oTicket.TicketGuid);
                    ObjTicket.Title = oTicket.Title;
                    ObjTicket.ProblemDescription = oTicket.ProblemDescription;
                    ObjTicket.CategoryId = oTicket.CategoryId;
                    ObjTicket.CurrentStatus = oTicket.CurrentStatus;
                    ObjTicket.OperatorPriority = oTicket.OperatorPriority;
                    ObjTicket.CustomerPriority = oTicket.CustomerPriority;
                    ObjTicket.AssignToOperator = oTicket.AssignToOperator;
                    ObjTicket.AssignToSupportTeam = oTicket.AssignToSupportTeam;

                    if (new ContractTemplateBL().IsSlaAllowOnContractById(ObjTicket.ContractId))
                    {
                        if (ObjTicket.ResponseDate == null)
                            ObjTicket.ResponseDate = DateTime.Now;

                    }

                    if (IsSetCloseStatus(ObjTicket))
                    {
                        ObjTicket.ClosedBy = UserId;
                        ObjTicket.ClosedOn = DateTime.Now;
                        if (new ContractTemplateBL().IsSlaAllowOnContractById(ObjTicket.ContractId))
                        {
                            if (ObjTicket.SolutionDate == null)
                                ObjTicket.SolutionDate = DateTime.Now;
                        }
                    }

                    ctx.Entry(ObjTicket).State = EntityState.Modified;
                    ctx.SaveChanges();
                    return ObjTicket;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete Support Ticket record by Id
        public void Delete(int id)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    using (var ctx = new HelpDeskEntities())
                    {
                        Ticket oTicket=new SupportTicketBL().GetById(id);
                        new CommentBL().Delete(oTicket.TicketGuid);
                        new AttachmentsBL().DeleteByLinkId(id, Constants.TicketImgUploadPath.ToString());
                        ctx.Entry(oTicket).State = EntityState.Deleted;
                        ctx.SaveChanges();
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
        }

        public string GetNextTicketNumber(string _TicketNumber)
        {
            string TicketViewNo = _TicketNumber;
            int TotalNumber = 7;
            int _AddnumberofZero = TotalNumber - TicketViewNo.Count();
            var TicketNumber = new StringBuilder();
            int NextTicketNumber = Convert.ToInt32(TicketViewNo) + 1;
            for (int i = 0; i < _AddnumberofZero; i++)
            {
                TicketNumber.Append("0");
            }
            TicketNumber.Append(NextTicketNumber.ToString());
            return TicketNumber.ToString();
        }

        public void AssignOperatorToTicket(int TicketId,int OperatorId)
        {
            try
            {
                using(var ctx = new HelpDeskEntities())
                {
                    ctx.Database.ExecuteSqlCommand("update Tickets set AssignToOperator = '" + OperatorId + "' where TicketId = '" + TicketId + "'");
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
