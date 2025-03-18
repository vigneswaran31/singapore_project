using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskEntity
{
    [MetadataType(typeof(TicketPriorityMetadata))]
    public partial class TicketPriority
    { }
    public class TicketPriorityMetadata
    {
        [Required(ErrorMessage = "Please enter Description.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter Sort Order.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Order No must be a Number.")]
        public int OrderByNo { get; set; }
    }
}
