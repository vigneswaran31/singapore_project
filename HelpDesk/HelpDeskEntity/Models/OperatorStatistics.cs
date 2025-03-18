using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    public  class OperatorStatistics
    {
        public int SLA_Total_Closed { get; set; }
        public int SLA_Closed_In { get; set; }
        public int SLA_Closed_Outof { get; set; }
        public int SLA_Performance_Percentage { get; set; }


        public int Overall_Total { get; set; }
        public int Overall_Closed { get; set; }
        public int Overall_Pending { get; set; }
        public int Overall_Pending_Percentage { get; set; }

        public int Assignto_Own_Total { get; set; }
        public int Assignto_Own_Closed { get; set; }
        public int Assignto_Own_Pending { get; set; }
        public int Assignto_Own_Percentage { get; set; }

        public int Percentage_wise_Total { get; set; }
        public int Percentage_wise_Closed { get; set; }
        public int Percentage_wise_Pending { get; set; }
    }

    public class StatisticsPiechart
    {
        public string label { get; set; }
        public int data { get; set; }
        public string color { get; set; }
    }

    public class AdminStatisticsPiechart
    {
        public string label { get; set; }
        public decimal data { get; set; }
        public string color { get; set; }
    }

    public class AdminTableData
    {
        public string Name { get; set; }
        public int Pending { get; set; }
        public int ClosedInSLA { get; set; }
        public int ClosedOutOfSLA { get; set; }
        public decimal SLAPerformance { get; set; }
        public dynamic PercentData { get; set; }

    }

    public class AdminStatisticsBarchart
    {
        public string label { get; set; }
        public int data { get; set; }
        public string color { get; set; }
    }
}
