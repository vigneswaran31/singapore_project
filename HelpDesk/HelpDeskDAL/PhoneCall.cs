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
    
    public partial class PhoneCall
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public string Phone { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Comment { get; set; }
        public Nullable<int> TicketId { get; set; }
        public Nullable<int> CallTime { get; set; }
        public Nullable<bool> Processed { get; set; }
    
        public virtual User User { get; set; }
    }
}
