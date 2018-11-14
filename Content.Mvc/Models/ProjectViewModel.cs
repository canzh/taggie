using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Content.Mvc.Models
{
    public class ProjectViewModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "BaseDir")]
        public string BaseDir { get; set; }

        [Display(Name = "TotalItems")]
        public int TotalItems { get; set; }

        [Display(Name = "Status")]
        public ProjectStatus Status { get; set; }

        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Assignments")]
        public List<ProjectAssignmentViewModel> Assignments { get; set; }

    }
}
