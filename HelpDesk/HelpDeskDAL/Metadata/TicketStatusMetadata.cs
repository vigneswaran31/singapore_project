using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{

    [MetadataType(typeof(TicketStatusMetadata))]
    public partial class TicketStatu
    { }
   
   public class TicketStatusMetadata
    {
        [Required(ErrorMessage = "Please enter Status Name.")]
        public string StatusName { get; set; }

        [Required(ErrorMessage = "Please select Icon Color.")]
        public string IconColor { get; set; }

        [Required(ErrorMessage = "Please enter Sort Order.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Order No must be a Number.")]
        public int OrderByNo { get; set; }

    }
}
