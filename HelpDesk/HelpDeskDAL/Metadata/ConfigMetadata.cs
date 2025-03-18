using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskEntity
{
    [MetadataType(typeof(ConfigMetadata))]
    public partial class Config
    {

    }
    public class ConfigMetadata
    {
         [Required(ErrorMessage = "Please enter Value")]
        public string Value { get; set; }
    }
}
