using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity.Classes
{
    public enum En_UserSession
    {
        User,
        SupportTeams,
        OperatorPermission,
        SupportTeamPermission,
        Role
    }

    public enum En_CRUD
    {
        Insert,
        Update,
        Delete
    }

    public enum En_Priority_Role
    {
        Operator = 1,//this is for Operator Priorities
        Customer = 2,//this is for Customer Priorities
    }

    public enum En_Role
    {
        Admin = 1,
        Operator = 2
    }

    public enum En_CompanyUserRole
    {
        CompanyUser = 3,
        CompanySuperUser = 4
    }

    public enum En_LinkType
    {
        Company = 1,
        KBArticle = 2,
        SupportTicket = 3
    }

    public enum En_PermissionType
    {
        Ticket,
        KBArticle,
        Chat
    }

    public enum En_Permission
    {
        CreateNewTickets = 2,
        ReOpenClosedTickets = 3,
        DeleteTickets = 5,
        ReassignOwnTickets = 13,
        ViewOthersTickets = 14,
        EditOthersTickets = 15,
        CreateArticle = 16,
        EditOtherArticle = 17,
        AllowSupportChat = 18
    }

    public enum En_Ticket_Status_Name 
    {
        Open,
        Closed
    }

    public enum En_Config
    {
        AttachmentType,
        MetaTitle,
        MetaDescription,
        MetaKeyword,
        Copyright,
        AdminEmail,
        TicketViewNumber,
        PortalTitle
    }

    public enum En_EmailKey
    {
        Creation_ByOperator,
        Creation_ByContact
    }
}
