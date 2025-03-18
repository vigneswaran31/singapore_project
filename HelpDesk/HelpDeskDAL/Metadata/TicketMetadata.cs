using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(TicketMetadata))]

    public partial class Ticket 
    { }


    public class TicketMetadata
    {
        [Required(ErrorMessage = "Please enter Title.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please select Company.")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Please select Contract.")]
        public int ContractId { get; set; }

        [Required(ErrorMessage = "Please select Category.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please Enter Problem Description.")]
        public string ProblemDescription { get; set; }

        [Required(ErrorMessage = "Please Select Customer Priority.")]
        public int CustomerPriority { get; set; }

        [Required(ErrorMessage = "Please Select Operator Priority.")]
        public int OperatorPriority { get; set; }
        [Required(ErrorMessage = "Please Select Company User.")]
        public int CompanyUserId { get; set; }

        [Required(ErrorMessage = "Please Select Status.")]
        public int CurrentStatus { get; set; }


    }
}
