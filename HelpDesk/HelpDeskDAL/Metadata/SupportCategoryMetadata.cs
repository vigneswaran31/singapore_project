using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(SupportCategoryMetadata))]
    public partial class SupportCategory
    {
    }

    public class SupportCategoryMetadata
    {
        [Required(ErrorMessage = "Please enter Category Name.")]
        public string CategoryName { get; set; }
    }
}
