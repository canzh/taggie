using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Content.Mvc.Models
{
    public class ProjectViewModel
    {
        [Display(Name = "ProjectId")]
        public int Id { get; set; }

        [Display(Name = "ProjectName")]
        public string ProjectName { get; set; }

        [Display(Name = "ProjectDescription")]
        public string Description { get; set; }

        [Display(Name = "ProjectBaseDir")]
        public string BaseDir { get; set; }

        [Display(Name = "TotalItems")]
        public int TotalItems { get; set; }

        [Display(Name = "ProjectStatus")]
        public ProjectStatus Status { get; set; }

        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Assignments")]
        public List<ProjectAssignmentViewModel> Assignments { get; set; }
    }
}
