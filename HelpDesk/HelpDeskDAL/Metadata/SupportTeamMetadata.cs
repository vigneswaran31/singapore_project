using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(SupportTeamMetadata))]
    public partial class SupportTeam
    {
    }

    public class SupportTeamMetadata
    {
        [Required(ErrorMessage = "Please enter Name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter Description.")]
        public string Description { get; set; }
    }
}
