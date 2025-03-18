using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(CompanyCategoryMetadata))]
    public partial class CompanyCategory
    {
    }

    public class CompanyCategoryMetadata
    {
        [Required(ErrorMessage="Please enter Name.")]
        public string Name { get; set; }
    }
}
