using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(TicketActivityMetadata))]
    public partial class TicketActivity
    {

    }

    public class TicketActivityMetadata
    {
        [Required(ErrorMessage = "Please enter Description.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter Note.")]
        public string Note { get; set; }

        [Required(ErrorMessage = "Please select FromDate.")]
        public System.DateTime FromDate { get; set; }

        [Required(ErrorMessage = "Please select ToDate.")]
        public System.DateTime ToDate { get; set; }
    }
}
