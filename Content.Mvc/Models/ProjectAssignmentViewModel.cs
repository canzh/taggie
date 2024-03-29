﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Content.Mvc.Models
{
    public class ProjectAssignmentViewModel
    {
        public int TeamId { get; set; }

        [Display(Name = "TeamName")]
        public string TeamName { get; set; }

        [Display(Name = "AssignedItemsCount")]
        public int AssignedItemsCount { get; set; }

        [Display(Name = "TeamList")]
        public List<SelectListItem> Teams { get; set; }
    }
}
