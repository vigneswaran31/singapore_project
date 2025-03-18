using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(TicketSourceMetadata))]
    public partial class TicketsSource
    {

    }

    public class TicketSourceMetadata
    {
        [Required(ErrorMessage = "Please enter Description.")]
        public string Description { get; set; }
    }
}
