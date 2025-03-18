using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace HelpDeskEntity
{
    [MetadataType(typeof(ContractTemplateMetadata))]
    public partial class ContractTemplate
    { 

    }

    public class ContractTemplateMetadata
    {

        [Required(ErrorMessage = "Please enter Template Name.")]
        public string TemplateName { get; set; }

        
        //[Required(ErrorMessage = "Please enter No Of Tickets.")]
        //[RegularExpression(@"^[0-9]+$", ErrorMessage = "No Of Tickets must be a Number.")]
        ////[Range(0, int.MaxValue, ErrorMessage = "Please enter valid Numeric Value")]
        //public int NoOfTickets { get; set; }


        //[Required(ErrorMessage = "Please enter Responce WithIn Hours.")]
        //[RegularExpression(@"^[0-9]+$", ErrorMessage = "Responce WithIn Hours must be a Number.")]
        //public int ResponseWithInHours { get; set; }

        //[Required(ErrorMessage = "Please enter Solution WithIn Hours Name.")]
        //[RegularExpression(@"^[0-9]+$", ErrorMessage = "Solution WithIn Hours must be a Number.")]
        //public int SolutionWithInHours { get; set; }
    }
}
