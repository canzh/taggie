using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Mvc.Services.DTO
{
    public class IdentityResponseModel
    {
        public string UserId { get; set; }

        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}
