using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(CommentMetadata))]
    public partial class Comment
    {

    }
    public class CommentMetadata
    {
        [Required(ErrorMessage = "Please enter Comment.")]
        public string CommentText { get; set; }
    }
}
