using HelpDesk.Classes;
using HelpDeskBAL;
using HelpDeskEntity;
using HelpDeskEntity.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HelpDesk.Areas.Admin.Controllers
{
    public class StatisticsController : AdminBaseController
    {
        //Get Statistics main page.
        public ActionResult Index()
        {
            return View();
        }

        //Get Statistics chart and Data by Categories
        public ActionResult TicketByCategories()
        {
            AdminStatisticsPiechart oPiecharts = new AdminStatisticsPiechart();

            List<vw_SupportTicket> lstTickets = new SupportTicketBL().GetAllTickets();
            List<AdminStatisticsPiechart> CategoriesWiseChartData = lstTickets.GroupBy(i => i.CategoryId)
                                                         .Select(i => new AdminStatisticsPiechart { label = i.First().CategoryName, data = i.Count() }).OrderBy(a => a.label).ToList();
            ViewBag.TicketByCategories = Newtonsoft.Json.JsonConvert.SerializeObject(CategoriesWiseChartData);

            List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
            List<AdminTableData> CategoriesTableData = lstTickets.GroupBy(i => i.CategoryId).OrderBy(i => i.First().CategoryName)
                                                       .Select(i => new AdminTableData
                                                       {
                                                           Name = i.First().CategoryName,
                                                           Pending = i.Where(k => lstOpenStatusId.Contains(k.CurrentStatusId)).ToList().Count(),
                                                           ClosedOutOfSLA = i.Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId) && k.ResponseDeadline != null && k.SolutionDeadline != null
                                                                                && (k.ResponseDate > k.ResponseDeadline || k.SolutionDate > k.SolutionDeadline)).ToList().Count(),
                                                           ClosedInSLA = i.Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId) && k.ResponseDeadline != null && k.SolutionDeadline != null
                                                                                && k.ResponseDate <= k.ResponseDeadline && k.SolutionDate <= k.SolutionDeadline).ToList().Count(),
                                                           SLAPerformance = i.Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId) && k.ResponseDeadline != null && k.SolutionDeadline != null).ToList().Count() > 0 ? (i.Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId) && k.ResponseDeadline != null && k.SolutionDeadline != null
                                                                                && k.ResponseDate <= k.ResponseDeadline && k.SolutionDate <= k.SolutionDeadline).ToList().Count() * 100 / (decimal)i.Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId) && k.ResponseDeadline != null && k.SolutionDeadline != null).ToList().Count()) : 0
                                                       }).ToList();

            return PartialView("_TicketByCategories", CategoriesTableData);
        }

        //Get Statistics chart and Data by operators
        public ActionResult TicketByOperators()
        {
            AdminStatisticsPiechart oPiecharts = new AdminStatisticsPiechart();

            List<vw_SupportTicket> lstTickets = new SupportTicketBL().GetAllTickets();
            List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();

            List<AdminStatisticsPiechart> OperatorsWiseChartData = lstTickets.Where(i => i.AssignToOperator != null).Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId)).GroupBy(i => i.AssignToOperator)
                                                         .Select(i => new AdminStatisticsPiechart { label = i.First().AssignToName, data = i.Count() }).OrderBy(a => a.label).ToList();
            ViewBag.TicketByOperators = Newtonsoft.Json.JsonConvert.SerializeObject(OperatorsWiseChartData);

            List<vw_SupportTicket> TotalOperaorsData = lstTickets.Where(i => i.AssignToOperator != null).ToList();
            List<AdminTableData> OperatorsTableData = lstTickets.Where(i => i.AssignToOperator != null).GroupBy(i => i.AssignToOperator).OrderBy(i => i.First().AssignToName)
                                                       .Select(i => new AdminTableData
                                                       {
                                                           Name = i.First().AssignToName,
                                                           Pending = i.Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId)).ToList().Count(),
                                                           ClosedOutOfSLA = i.Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId) && k.ResponseDeadline != null && k.SolutionDeadline != null
                                                                                && (k.ResponseDate > k.ResponseDeadline || k.SolutionDate > k.SolutionDeadline)).ToList().Count(),
                                                           ClosedInSLA = i.Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId) && k.ResponseDeadline != null && k.SolutionDeadline != null
                                                                                && k.ResponseDate <= k.ResponseDeadline && k.SolutionDate <= k.SolutionDeadline).ToList().Count(),
                                                           SLAPerformance = i.Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId) && k.ResponseDeadline != null && k.SolutionDeadline != null).ToList().Count() > 0 ? (i.Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId) && k.ResponseDeadline != null && k.SolutionDeadline != null
                                                                                && k.ResponseDate <= k.ResponseDeadline && k.SolutionDate <= k.SolutionDeadline).ToList().Count() * 100 / (decimal)i.Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId) && k.ResponseDeadline != null && k.SolutionDeadline != null).ToList().Count()) : 0
                                                       }).ToList();
            return PartialView("_TicketByOperators", OperatorsTableData);
        }

        //Get Statistics chart and Data by Operator Priorities
        public ActionResult TicketByOptPriority()
        {
            AdminStatisticsPiechart oPiecharts = new AdminStatisticsPiechart();
            List<int> lstOptPriorities = new TicketPriorityBL().GetPrioritiesByType(Convert.ToInt16(En_Priority_Role.Operator)).Select(i => i.TicketPriorityId).ToList();
            List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
            List<vw_SupportTicket> lstTickets = new SupportTicketBL().GetAllTickets();
            List<AdminStatisticsPiechart> OptPriorityWiseChartData = lstTickets.Where(i => lstOptPriorities.Contains(i.OperatorPriorityId))
                                                        .Where(k => lstOpenStatusId.Contains(k.CurrentStatusId)).GroupBy(i => i.OperatorPriorityId)
                                                         .Select(i => new AdminStatisticsPiechart { label = i.First().OperatorPriority, data = i.Count() }).OrderBy(a => a.label).ToList();
            ViewBag.TicketByOptPriority = Newtonsoft.Json.JsonConvert.SerializeObject(OptPriorityWiseChartData);

            List<AdminTableData> OptPriorityTableData = lstTickets.Where(i => lstOptPriorities.Contains(i.OperatorPriorityId))
                                                        .Where(k => lstOpenStatusId.Contains(k.CurrentStatusId)).GroupBy(i => i.OperatorPriorityId).OrderBy(i => i.First().OperatorPriority)
                                                       .Select(i => new AdminTableData
                                                       {
                                                           Name = i.First().OperatorPriority,
                                                           Pending = i.ToList().Count(),
                                                           SLAPerformance = i.ToList().Count * 100 / (decimal)lstTickets.Where(k => lstOpenStatusId.Contains(k.CurrentStatusId)).Count()
                                                       }).ToList();

            return PartialView("_TicketByOptPriority", OptPriorityTableData);
        }

        //Get Statistics chart and Data by Customer Priorities
        public ActionResult TicketByCusPriority()
        {
            AdminStatisticsPiechart oPiecharts = new AdminStatisticsPiechart();
            List<int> lstCusPriorities = new TicketPriorityBL().GetPrioritiesByType(Convert.ToInt16(En_Priority_Role.Customer)).Select(i => i.TicketPriorityId).ToList();
            List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
            List<vw_SupportTicket> lstTickets = new SupportTicketBL().GetAllTickets();
            List<AdminStatisticsPiechart> CusPriorityWiseChartData = lstTickets.Where(i => lstCusPriorities.Contains(i.CustomerPriorityId))
                                                            .Where(k => lstOpenStatusId.Contains(k.CurrentStatusId)).GroupBy(i => i.CustomerPriorityId)
                                                         .Select(i => new AdminStatisticsPiechart { label = i.First().CustomerPriority, data = i.Count() }).OrderBy(a => a.label).ToList();
            ViewBag.TicketByCusPriority = Newtonsoft.Json.JsonConvert.SerializeObject(CusPriorityWiseChartData);

            List<AdminTableData> CusPriorityWiseTableData = lstTickets.Where(i => lstCusPriorities.Contains(i.CustomerPriorityId))
                                                        .Where(k => lstOpenStatusId.Contains(k.CurrentStatusId)).GroupBy(i => i.CustomerPriorityId).OrderBy(i => i.First().CustomerPriority)
                                                       .Select(i => new AdminTableData
                                                       {
                                                           Name = i.First().CustomerPriority,
                                                           Pending = i.ToList().Count(),
                                                           SLAPerformance = i.ToList().Count * 100 / (decimal)lstTickets.Where(k => lstOpenStatusId.Contains(k.CurrentStatusId)).Count()
                                                       }).ToList();

            return PartialView("_TicketByCusPriority", CusPriorityWiseTableData);
        }

        //Get Statistics chart and Data by Status
        public ActionResult TicketByStatus()
        {
            AdminStatisticsPiechart oPiecharts = new AdminStatisticsPiechart();
            List<vw_SupportTicket> lstTickets = new SupportTicketBL().GetAllTickets();
            List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
            List<AdminStatisticsPiechart> OpenStatusWiseChartData = lstTickets.Where(i => lstOpenStatusId.Contains(i.CurrentStatusId)).GroupBy(i => i.CurrentStatusId)
                                                         .Select(i => new AdminStatisticsPiechart { label = i.First().StatusName, data = i.Count() }).OrderBy(a => a.label).ToList();
            List<AdminStatisticsPiechart> CloseStatusWiseChartData = lstTickets.Where(i => !lstOpenStatusId.Contains(i.CurrentStatusId)).GroupBy(i => i.CurrentStatusId)
                                                        .Select(i => new AdminStatisticsPiechart { label = i.First().StatusName, data = i.Count() }).OrderBy(a => a.label).ToList();
            ViewBag.TicketByOpenStatus = Newtonsoft.Json.JsonConvert.SerializeObject(OpenStatusWiseChartData);
            ViewBag.TicketByCloseStatus = Newtonsoft.Json.JsonConvert.SerializeObject(CloseStatusWiseChartData);

            List<vw_SupportTicket> TotalOpenStatusData = lstTickets.Where(i => lstOpenStatusId.Contains(i.CurrentStatusId)).ToList();
            List<vw_SupportTicket> TotalClosedStatusData = lstTickets.Where(i => !lstOpenStatusId.Contains(i.CurrentStatusId)).ToList();


            List<AdminTableData> OpenStatusWiseTableData = lstTickets.Where(i => lstOpenStatusId.Contains(i.CurrentStatusId)).GroupBy(i => i.CurrentStatusId).OrderBy(i => i.First().StatusName)
                                                       .Select(i => new AdminTableData
                                                       {
                                                           Name = i.First().StatusName,
                                                           Pending = i.ToList().Count(),
                                                           SLAPerformance = (i.ToList().Count * 100 / (decimal)TotalOpenStatusData.Count())
                                                       }).ToList();


            List<AdminTableData> ClosedStatusWiseTableData = lstTickets.Where(i => !lstOpenStatusId.Contains(i.CurrentStatusId)).GroupBy(i => i.CurrentStatusId).OrderBy(i => i.First().StatusName)
                                                      .Select(i => new AdminTableData
                                                      {
                                                          Name = i.First().StatusName,
                                                          Pending = i.ToList().Count(),
                                                          SLAPerformance = (i.ToList().Count * 100 / (decimal)TotalClosedStatusData.Count())
                                                      }).ToList();

            ViewBag.OpenStatusData = OpenStatusWiseTableData;
            ViewBag.ClosedStatusData = ClosedStatusWiseTableData;
            return PartialView("_TicketByStatus");
        }

        //Get Statistics chart and Data by Teams
        public ActionResult TicketByTeams()
        {
            AdminStatisticsPiechart oPiecharts = new AdminStatisticsPiechart();

            List<vw_SupportTicket> lstTickets = new SupportTicketBL().GetAllTickets();
            List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();

            List<AdminStatisticsPiechart> TeamsWiseChartData = lstTickets.Where(i => i.AssignToSupportTeam != null && lstOpenStatusId.Contains(i.CurrentStatusId)).GroupBy(i => i.AssignToSupportTeam)
                                                         .Select(i => new AdminStatisticsPiechart { label = i.First().AssignToName, data = i.Count() }).OrderBy(a => a.label).ToList();
            ViewBag.TicketByTeams = Newtonsoft.Json.JsonConvert.SerializeObject(TeamsWiseChartData);


            List<vw_SupportTicket> TotalTeamsData = lstTickets.Where(i => i.AssignToSupportTeam != null && lstOpenStatusId.Contains(i.CurrentStatusId)).ToList();
            List<AdminTableData> CategoriesWiseTableData = lstTickets.Where(i => i.AssignToSupportTeam != null && lstOpenStatusId.Contains(i.CurrentStatusId)).GroupBy(i => i.AssignToSupportTeam).OrderBy(i => i.First().AssignToSupportTeam)
                                                       .Select(i => new AdminTableData
                                                       {
                                                           Name = i.First().AssignToName,
                                                           Pending = i.Where(k => lstOpenStatusId.Contains(k.CurrentStatusId)).ToList().Count(),
                                                           SLAPerformance = i.ToList().Count * 100 / (decimal)TotalTeamsData.Count()
                                                       }).ToList();

            return PartialView("_TicketByTeams", CategoriesWiseTableData);
        }

        //Get Statistics chart and Data by Companies
        public ActionResult TicketByCompanies()
        {
            AdminStatisticsPiechart oPiecharts = new AdminStatisticsPiechart();
            List<vw_SupportTicket> lstTickets = new SupportTicketBL().GetAllTickets();
            List<int> lstCompanyContractId = new CompanyContractBL().GetActiveContractCompanies_ContractIds();
            List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
            List<vw_SupportTicket> lstActiveComapnyContract = lstTickets.Where(i => lstCompanyContractId.Contains(i.ContractId)).ToList();

            List<AdminStatisticsPiechart> OpenStatusWiseChartData = lstActiveComapnyContract.Where(i => lstOpenStatusId.Contains(i.CurrentStatusId)).GroupBy(i => i.CompanyId)
                                                         .Select(i => new AdminStatisticsPiechart
                                                         {
                                                             label = i.First().CompanyName + " (" + i.First().ContractName + ")",
                                                             data = i.Count()
                                                         }).OrderBy(a => a.label).ToList();


            List<AdminStatisticsPiechart> CloseStatusWiseChartData = lstActiveComapnyContract.Where(i => !lstOpenStatusId.Contains(i.CurrentStatusId)).GroupBy(i => i.CompanyId)
                                                        .Select(i => new AdminStatisticsPiechart
                                                        {
                                                            label = i.First().CompanyName + " (" + i.First().ContractName + ")",
                                                            data = i.Count()
                                                        }).OrderBy(a => a.label).ToList();


            ViewBag.TicketByOpenStatus = Newtonsoft.Json.JsonConvert.SerializeObject(OpenStatusWiseChartData);
            ViewBag.TicketByCloseStatus = Newtonsoft.Json.JsonConvert.SerializeObject(CloseStatusWiseChartData);

            List<AdminTableData> CompanyCurrentContractData = lstActiveComapnyContract.GroupBy(i => i.CompanyId).OrderBy(i => i.First().CompanyName)
                                                       .Select(i => new AdminTableData
                                                       {
                                                           Name = i.First().CompanyName + " (" + i.First().ContractName + ")",
                                                           Pending = i.Where(k => lstOpenStatusId.Contains(k.CurrentStatusId)).ToList().Count(),
                                                           ClosedInSLA = i.Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId)).ToList().Count(),
                                                       }).ToList();

            return PartialView("_TicketByCompanies", CompanyCurrentContractData);
        }

        //Get Statistics chart and Data by Companies Historic 5 years
        public ActionResult TicketByCompaniesHistoric()
        {
            AdminStatisticsPiechart oPiecharts = new AdminStatisticsPiechart();
            List<vw_SupportTicket> lstTickets = new SupportTicketBL().GetAllTickets();
            List<int> lstOpenStatusId = new TicketStatusBL().GetAllOpenStatus().Select(t => t.TicketStatusId).ToList();
            //DateTime PrvsFveYrsDate = DateTime.Now.AddYears(-5);
            int PrevFiveYear = DateTime.Now.AddYears(-5).Year;

            List<AdminStatisticsPiechart> OpenStatusWiseChartData = lstTickets.Where(i => lstOpenStatusId.Contains(i.CurrentStatusId) && i.CreatedOn.Value.Year >= PrevFiveYear && i.CreatedOn.Value.Year <= DateTime.Now.Year).GroupBy(i => i.CompanyId)
                                                         .Select(i => new AdminStatisticsPiechart
                                                         {
                                                             label = i.First().CompanyName,
                                                             data = i.Count()
                                                         }).OrderBy(a => a.label).ToList();


            List<AdminStatisticsPiechart> CloseStatusWiseChartData = lstTickets.Where(i => !lstOpenStatusId.Contains(i.CurrentStatusId) && i.CreatedOn.Value.Year >= PrevFiveYear && i.CreatedOn.Value.Year <= DateTime.Now.Year).GroupBy(i => i.CompanyId)
                                                        .Select(i => new AdminStatisticsPiechart
                                                        {
                                                            label = i.First().CompanyName,
                                                            data = i.Count()
                                                        }).OrderBy(a => a.label).ToList();


            ViewBag.TicketByOpenStatus = Newtonsoft.Json.JsonConvert.SerializeObject(OpenStatusWiseChartData);
            ViewBag.TicketByCloseStatus = Newtonsoft.Json.JsonConvert.SerializeObject(CloseStatusWiseChartData);

            List<AdminTableData> CompanyHistoricData = lstTickets.Where(i => i.CreatedOn.Value.Year >= PrevFiveYear && i.CreatedOn.Value.Year <= DateTime.Now.Year).GroupBy(i => i.CompanyId).OrderBy(i => i.First().CompanyName)
                                                       .Select(i => new AdminTableData
                                                       {
                                                           Name = i.First().CompanyName,
                                                           ClosedOutOfSLA = i.Select(k => k.ContractId).Distinct().Count(),
                                                           Pending = i.Where(k => lstOpenStatusId.Contains(k.CurrentStatusId)).ToList().Count(),
                                                           ClosedInSLA = i.Where(k => !lstOpenStatusId.Contains(k.CurrentStatusId)).ToList().Count(),
                                                       }).ToList();

            return PartialView("_TicketByCompaniesHistoric", CompanyHistoricData);
        }


        public ActionResult Value()
        {
            //Bar chart - 5 yearly contract price
            AdminStatisticsBarchart oBarcharts = new AdminStatisticsBarchart();
            List<vw_CompanyContract> lstCompanyContracts = new CompanyContractBL().GetAllCompanyContracts();
            DateTime PrevFiveYear = DateTime.Now.AddYears(-5).Date;
            int CurrentYear = DateTime.Now.Year;

            List<AdminStatisticsBarchart> ValuesForYearData = lstCompanyContracts.Where(i => i.StartDate >= PrevFiveYear && i.StartDate <= DateTime.Now.Date).GroupBy(i => i.StartDate.Year)
                 .Select(i => new AdminStatisticsBarchart
                 {
                     label = i.First().StartDate.Year.ToString(),
                     data = Convert.ToInt32(i.Sum(t => t.Price))
                 }).OrderBy(a => a.label).ToList();

            ArrayList arrValuesForYearData = new ArrayList();
            foreach (AdminStatisticsBarchart item in ValuesForYearData)
            {
                string[] array = new string[] { item.label, item.data.ToString() };
                arrValuesForYearData.Add(array);
            }
            ViewBag.ValuesForEachYear = new JavaScriptSerializer().Serialize(arrValuesForYearData);

            //Pie chart - yearly contract 
            List<string> lstYears = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                lstYears.Add(DateTime.Now.AddYears(-i).Year.ToString());
            }
            ViewBag.Years = lstYears;

            AdminStatisticsPiechart oPiecharts = new AdminStatisticsPiechart();
            lstCompanyContracts = lstCompanyContracts.Where(c => c.CategoryId > 0).ToList();//to avoid old records with category 0 or null

            List<AdminStatisticsBarchart> ValuesbyCatgry = lstCompanyContracts.Where(i => i.StartDate.Year == CurrentYear).GroupBy(i => i.CategoryId)
               .Select(i => new AdminStatisticsBarchart
               {
                   label = i.First().CategoryName.ToString(),
                   data = Convert.ToInt16(i.Sum(t => t.Price))
               }).OrderBy(a => a.label).ToList();

            ViewBag.ValuesbyCatgry = Newtonsoft.Json.JsonConvert.SerializeObject(ValuesbyCatgry);

            return View();
        }

        public ActionResult GetValueByYear(int Year)
        {
            List<vw_CompanyContract> lstCompCatgry = new CompanyContractBL().GetAllCompanyContracts().Where(c => c.CategoryId > 0).ToList();//to avoid old records with category 0 or null

            List<AdminStatisticsBarchart> ValuesbyCatgry = lstCompCatgry.Where(i => i.StartDate.Year == Year).GroupBy(i => i.CategoryId)
               .Select(i => new AdminStatisticsBarchart
               {
                   label = i.First().CategoryName.ToString(),
                   data = Convert.ToInt16(i.Sum(t => t.Price))
               }).OrderBy(a => a.label).ToList();

            ViewBag.ValuesbyCatgry = Newtonsoft.Json.JsonConvert.SerializeObject(ValuesbyCatgry);
            return PartialView("_ValuesByYear");
        }

    }
}