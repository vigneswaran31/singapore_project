using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(LanguageMetadata))]
    public partial class Language
    {
    }

    public class LanguageMetadata
    {
        [Required(ErrorMessage = "Please enter Language.")]
        public string Description { get; set; }
    }
}
