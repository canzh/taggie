using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace taggie.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Role")]
        public string ApplicationRoleId { get; set; }

        public List<SelectListItem> ApplicationRoles { get; set; }

    }
}
