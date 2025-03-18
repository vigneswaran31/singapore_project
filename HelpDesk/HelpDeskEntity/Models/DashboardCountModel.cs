using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    public class DashboardCountModel
    {
        public int SupportTickets { get; set; }
        public int SupportCategories { get; set; }
        public int Admins { get; set; }
        public int Operators { get; set; }
        public int SupportTeams { get; set; }
        public int Companies { get; set; }
        public int KBCategories { get; set; }
        public int KBArticles { get; set; }

        public int EmailInbox { get; set; }
    }

    public class OperatorDashboardCount
    {
        public int AssignedToMe_Below_fifty { get; set; }
        public int AssignedToMe_Beetween_Fifty_Eighty { get; set; }
        public int AssignedToMe_Above_Eighty{ get; set; }

        public int AssignedToTeams_Below_fifty { get; set; }
        public int AssignedToTeams_Beetween_Fifty_Eighty { get; set; }
        public int AssignedToTeams_Above_Eighty { get; set; }

        public int UnassignedTickets_Below_fifty { get; set; }
        public int UnassignedTickets_Beetween_Fifty_Eighty { get; set; }
        public int UnassignedTickets_Above_Eighty { get; set; }
    }
}
