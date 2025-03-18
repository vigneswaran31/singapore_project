using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(EmailTemplateMetadata))]
    public partial class EmailTemplate
    {

    }
    public class EmailTemplateMetadata
    {
        [Required(ErrorMessage = "Please select Language.")]
        public Nullable<int> LanguageId { get; set; }

        [Required(ErrorMessage = "Please enter Subject.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter Body.")]
        public string Body { get; set; }
    }
}
