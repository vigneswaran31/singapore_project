using HelpDesk.Classes;
using HelpDeskEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpDeskBAL;

namespace HelpDesk.Areas.Account.Controllers
{
    public class StatisticsController : BaseController
    {
        //
        // GET: /Account/Statistics/

        public ActionResult Index()
        {

            return View();
        }

        //Load all Statistics Data
        public ActionResult Data()
        {
            OperatorStatistics oOperatorStatistics = new OperatorStatistics();
            User oUser = SiteUtility.GetCurrentUser();
            List<int> lstTeamId = new SupportTeamMembersBL().GetAllTeamByMemberId(oUser.UserId).Select(t => t.TeamId).ToList();
            List<vw_SupportTicket> lstTickets = new SupportTicketBL().GetAllTickets();
            if (lstTickets != null && lstTickets.Count > 0)
            {
                List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
                List<vw_SupportTicket> lst_Operator_All = lstTickets.Where(t => t.AssignToOperator == oUser.UserId || lstTeamId.Contains(t.AssignToSupportTeam != null ? t.AssignToSupportTeam.Value : 0)).ToList();
                List<vw_SupportTicket> lstClosed_Tickets = lstTickets.Where(t => !lstOpenStatusId.Contains(t.CurrentStatusId)).ToList();

                List<vw_SupportTicket> lst_Operator_Closed = lstClosed_Tickets.Where(t => t.AssignToOperator == oUser.UserId || lstTeamId.Contains(t.AssignToSupportTeam != null ? t.AssignToSupportTeam.Value : 0)).ToList();
                List<vw_SupportTicket> lst_Operator_Closed_SLA = lst_Operator_Closed.Where(t => t.ResponseDeadline != null && t.SolutionDeadline != null).ToList();

                //For SLA
                oOperatorStatistics.SLA_Total_Closed = lst_Operator_Closed_SLA.Count();
                oOperatorStatistics.SLA_Closed_In = lst_Operator_Closed_SLA.Where(t => t.ResponseDate <= t.ResponseDeadline && t.SolutionDate <= t.SolutionDeadline).Count();
                oOperatorStatistics.SLA_Closed_Outof = oOperatorStatistics.SLA_Total_Closed - oOperatorStatistics.SLA_Closed_In;
                if (oOperatorStatistics.SLA_Total_Closed > 0)
                    oOperatorStatistics.SLA_Performance_Percentage = oOperatorStatistics.SLA_Closed_In * 100 / oOperatorStatistics.SLA_Total_Closed;

                //For Over All
                oOperatorStatistics.Overall_Total = lstTickets.Count();
                oOperatorStatistics.Overall_Closed = lstClosed_Tickets.Count();
                oOperatorStatistics.Overall_Pending = oOperatorStatistics.Overall_Total - oOperatorStatistics.Overall_Closed;

                if (oOperatorStatistics.Overall_Total > 0)
                    oOperatorStatistics.Overall_Pending_Percentage = oOperatorStatistics.Overall_Pending * 100 / oOperatorStatistics.Overall_Total;


                //For Assign to Own
                oOperatorStatistics.Assignto_Own_Total = lst_Operator_All.Count();
                oOperatorStatistics.Assignto_Own_Closed = lst_Operator_Closed.Count();
                oOperatorStatistics.Assignto_Own_Pending = oOperatorStatistics.Assignto_Own_Total - oOperatorStatistics.Assignto_Own_Closed;

                if (oOperatorStatistics.Assignto_Own_Total > 0)
                    oOperatorStatistics.Assignto_Own_Percentage = oOperatorStatistics.Assignto_Own_Pending * 100 / oOperatorStatistics.Assignto_Own_Total;

                //For Percentage
                if (oOperatorStatistics.Overall_Total > 0)
                    oOperatorStatistics.Percentage_wise_Total = oOperatorStatistics.Assignto_Own_Total * 100 / oOperatorStatistics.Overall_Total;

                if (oOperatorStatistics.Overall_Closed > 0)
                    oOperatorStatistics.Percentage_wise_Closed = oOperatorStatistics.Assignto_Own_Closed * 100 / oOperatorStatistics.Overall_Closed;

                if (oOperatorStatistics.Overall_Pending > 0)
                    oOperatorStatistics.Percentage_wise_Pending = oOperatorStatistics.Assignto_Own_Pending * 100 / oOperatorStatistics.Overall_Pending;

            }

            return PartialView("_StatisticsData", oOperatorStatistics);
        }

        //Load all Charts
        public ActionResult Charts()
        {
            OperatorStatistics oOperatorStatistics = new OperatorStatistics();
            User oUser = SiteUtility.GetCurrentUser();
            List<int> lstTeamId = new SupportTeamMembersBL().GetAllTeamByMemberId(oUser.UserId).Select(t => t.TeamId).ToList();
            List<vw_SupportTicket> lstTickets = new SupportTicketBL().GetAllTickets();
            if (lstTickets != null && lstTickets.Count > 0)
            {
                List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
                List<vw_SupportTicket> lst_Operator_All = lstTickets.Where(t => t.AssignToOperator == oUser.UserId || lstTeamId.Contains(t.AssignToSupportTeam != null ? t.AssignToSupportTeam.Value : 0)).ToList();
                List<vw_SupportTicket> lstClosed_Tickets = lstTickets.Where(t => !lstOpenStatusId.Contains(t.CurrentStatusId)).ToList();

                List<vw_SupportTicket> lst_Operator_Closed = lstClosed_Tickets.Where(t => t.AssignToOperator == oUser.UserId || lstTeamId.Contains(t.AssignToSupportTeam != null ? t.AssignToSupportTeam.Value : 0)).ToList();
                List<vw_SupportTicket> lst_Operator_Closed_SLA = lst_Operator_Closed.Where(t => t.ResponseDeadline != null && t.SolutionDeadline != null).ToList();

                //For SLA
                oOperatorStatistics.SLA_Total_Closed = lst_Operator_Closed_SLA.Count();
                oOperatorStatistics.SLA_Closed_In = lst_Operator_Closed_SLA.Where(t => t.ResponseDate <= t.ResponseDeadline && t.SolutionDate <= t.SolutionDeadline).Count();
                oOperatorStatistics.SLA_Closed_Outof = oOperatorStatistics.SLA_Total_Closed - oOperatorStatistics.SLA_Closed_In;
                if (oOperatorStatistics.SLA_Total_Closed > 0)
                    oOperatorStatistics.SLA_Performance_Percentage = oOperatorStatistics.SLA_Closed_In * 100 / oOperatorStatistics.SLA_Total_Closed;

                //For Over All
                oOperatorStatistics.Overall_Total = lstTickets.Count();
                oOperatorStatistics.Overall_Closed = lstClosed_Tickets.Count();
                oOperatorStatistics.Overall_Pending = oOperatorStatistics.Overall_Total - oOperatorStatistics.Overall_Closed;

                if (oOperatorStatistics.Overall_Total > 0)
                    oOperatorStatistics.Overall_Pending_Percentage = oOperatorStatistics.Overall_Pending * 100 / oOperatorStatistics.Overall_Total;


                //For Assign to Own
                oOperatorStatistics.Assignto_Own_Total = lst_Operator_All.Count();
                oOperatorStatistics.Assignto_Own_Closed = lst_Operator_Closed.Count();
                oOperatorStatistics.Assignto_Own_Pending = oOperatorStatistics.Assignto_Own_Total - oOperatorStatistics.Assignto_Own_Closed;

                if (oOperatorStatistics.Assignto_Own_Total > 0)
                    oOperatorStatistics.Assignto_Own_Percentage = oOperatorStatistics.Assignto_Own_Pending * 100 / oOperatorStatistics.Assignto_Own_Total;

                //For Percentage
                if (oOperatorStatistics.Overall_Total > 0)
                    oOperatorStatistics.Percentage_wise_Total = oOperatorStatistics.Assignto_Own_Total * 100 / oOperatorStatistics.Overall_Total;

                if (oOperatorStatistics.Overall_Closed > 0)
                    oOperatorStatistics.Percentage_wise_Closed = oOperatorStatistics.Assignto_Own_Closed * 100 / oOperatorStatistics.Overall_Closed;

                if (oOperatorStatistics.Overall_Pending > 0)
                    oOperatorStatistics.Percentage_wise_Pending = oOperatorStatistics.Assignto_Own_Pending * 100 / oOperatorStatistics.Overall_Pending;

            }

            List<StatisticsPiechart> lstoperatorPerformance = new List<StatisticsPiechart>();
            lstoperatorPerformance.Add(new StatisticsPiechart { label = "Closed in SLA", data = oOperatorStatistics.SLA_Closed_In, color = "#DA5430" });
            lstoperatorPerformance.Add(new StatisticsPiechart { label = "Closed out of SLA", data = oOperatorStatistics.SLA_Closed_Outof, color = "#68BC31" });
            ViewBag.SLAPerformance = Newtonsoft.Json.JsonConvert.SerializeObject(lstoperatorPerformance);

            List<StatisticsPiechart> lstAssigned_to_own = new List<StatisticsPiechart>();
            //lstAssigned_to_own.Add(new StatisticsPiechart { label = "Assigned To you", data = oOperatorStatistics.Assignto_Own_Total, color = "#DA5430" });
            //lstAssigned_to_own.Add(new StatisticsPiechart { label = "Not Assigned to you", data = oOperatorStatistics.Overall_Total, color = "#68BC31" });
            lstAssigned_to_own.Add(new StatisticsPiechart { label = "Assigned To you", data = oOperatorStatistics.Percentage_wise_Total, color = "#DA5430" });
            lstAssigned_to_own.Add(new StatisticsPiechart { label = "Not Assigned to you", data = 100 - oOperatorStatistics.Percentage_wise_Total, color = "#68BC31" });
            ViewBag.Assigned_to_own = Newtonsoft.Json.JsonConvert.SerializeObject(lstAssigned_to_own);

            List<StatisticsPiechart> lstOwn_Closed = new List<StatisticsPiechart>();
            lstOwn_Closed.Add(new StatisticsPiechart { label = "Closed by you", data = oOperatorStatistics.Percentage_wise_Closed, color = "#DA5430" });
            lstOwn_Closed.Add(new StatisticsPiechart { label = "Closed by other", data = 100 - oOperatorStatistics.Percentage_wise_Closed, color = "#68BC31" });
            ViewBag.Own_Closed = Newtonsoft.Json.JsonConvert.SerializeObject(lstOwn_Closed);


            List<StatisticsPiechart> lstOwn_Pending = new List<StatisticsPiechart>();
            lstOwn_Pending.Add(new StatisticsPiechart { label = "Your pending", data = oOperatorStatistics.Percentage_wise_Pending, color = "#DA5430" });
            lstOwn_Pending.Add(new StatisticsPiechart { label = "Others pending", data = 100 - oOperatorStatistics.Percentage_wise_Pending, color = "#68BC31" });
            ViewBag.Own_Pending = Newtonsoft.Json.JsonConvert.SerializeObject(lstOwn_Pending);

            return PartialView("_StatisticsChart", oOperatorStatistics);
        }
	}
}