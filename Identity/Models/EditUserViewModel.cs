using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace taggie.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [Display(Name = "Role")]
        public string ApplicationRoleId { get; set; }

        // For UI interaction
        public List<SelectListItem> ApplicationRoles { get; set; }
    }
}
