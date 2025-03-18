using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{

    [MetadataType(typeof(CompanyContractMetadata))]
    public partial class CompanyContract
    {

    }
    class CompanyContractMetadata
    {
        [Required(ErrorMessage = "Please select Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Please select Contract Template")]
        public int ContractId { get; set; }

        [Required(ErrorMessage = "Please enter Contract Number")]
        public int ContractNumber { get; set; }

        [Required(ErrorMessage = "Please enter Start Date")]
        public System.DateTime StartDate { get; set; }

         [Required(ErrorMessage = "Please enter End Date")]
        public System.DateTime EndDate { get; set; }
    }
}
