using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(SupportTeamMemebersMetadata))]
    public partial class SupportTeamMember
    {

    }
    public class SupportTeamMemebersMetadata
    {
        [Required(ErrorMessage = "Please select Users.")]
        public int UserId { get; set; }
    }
}
