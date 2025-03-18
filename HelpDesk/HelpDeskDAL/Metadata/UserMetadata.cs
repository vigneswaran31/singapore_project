using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HelpDeskEntity
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {

    }
    public class UserMetadata
    {
        [Required(ErrorMessage = "Please select Role.")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Please enter Name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter Email.")]
        [Remote("checkEmail", "Users","Admin", AdditionalFields = "UserId", HttpMethod = "POST", ErrorMessage = "Email address is already in use by another user. Please use a different e-mail address")]
     
        public string Email { get; set; }

      //  [Required(ErrorMessage = "Please enter Password.")]
        public string Password { get; set; }

        //[Required(ErrorMessage = "Please enter Contact.")]
        //public string Contact { get; set; }
    }
}
