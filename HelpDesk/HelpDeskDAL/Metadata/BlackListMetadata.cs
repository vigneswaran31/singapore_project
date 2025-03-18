using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(BlackListMetadata))]
    public partial class EmailBlackList
    {
    }

    public class BlackListMetadata
    {
        [Required(ErrorMessage = "Please enter MailAddress.")]
        [RegularExpression("([-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\\.[a-zA-Z]{2,4})", ErrorMessage = "Please enter valid Email")]
        public string MailAddress { get; set; }
    }
}
