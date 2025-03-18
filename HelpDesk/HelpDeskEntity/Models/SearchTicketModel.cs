using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity.Models
{
    public class SearchTicketModel
    {
        public string ProblemDescription { get; set; }
        public int Status { get; set; }

        public int OperatorPriority { get; set; }
        public int CustomerPriority { get; set; }
        public int Category { get; set; }
        public int CompanyUser { get; set; }
        public int Company { get; set; }
        public int AssignedToOperator { get; set; }
        public int AssignedToSupportTeam { get; set; }
        public Nullable<System.DateTime> DateTicketFrom { get; set; }
        public Nullable<System.DateTime> DateTicketTo { get; set; }

    }

    public class UserSearchTicketModel
    {
        public string ProblemDescription { get; set; }
        public int Status { get; set; }
        public int CustomerPriority { get; set; }
        public int Category { get; set; }
        public int CompanyUser { get; set; }
        public Nullable<System.DateTime> DateTicketFrom { get; set; }
        public Nullable<System.DateTime> DateTicketTo { get; set; }

    }
}
