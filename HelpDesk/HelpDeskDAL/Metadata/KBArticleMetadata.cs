using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(KBArticleMetadata))]
    public partial class KBArticle
    {

    }
    public class KBArticleMetadata
    {
        [Required(ErrorMessage = "Please select Category")]
        public int KBCategoryId { get; set; }

        [Required(ErrorMessage = "Please enter Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter Description.")]
        public string Description { get; set; }
    }
}
