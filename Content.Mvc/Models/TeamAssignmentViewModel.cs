using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Content.Mvc.Models
{
    public class TeamAssignmentViewModel
    {
        [Display(Name = "ProjectName")]
        public string ProjectName { get; set; }

        [Display(Name = "AssignedItemsCount")]
        public int AssignedItemsCount { get; set; }
    }
}
