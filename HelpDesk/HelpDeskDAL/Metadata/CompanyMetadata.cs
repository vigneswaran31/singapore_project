using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(CompanyMetadata))]
    public partial class Company
    {
 
    }

    public class CompanyMetadata
    {
        [Required(ErrorMessage = "Please enter Company Name.")]
        public string CompanyName { get; set; }

    }
}
