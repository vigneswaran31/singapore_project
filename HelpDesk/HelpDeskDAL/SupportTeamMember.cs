//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HelpDeskEntity
{
    using System;
    using System.Collections.Generic;
    
    public partial class SupportTeamMember
    {
        public int TeamId { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedOn { get; set; }
    
        public virtual SupportTeam SupportTeam { get; set; }
        public virtual User User { get; set; }
    }
}
