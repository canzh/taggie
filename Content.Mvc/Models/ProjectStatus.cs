using System.ComponentModel.DataAnnotations;

namespace Content.Mvc.Models
{
    public enum ProjectStatus
    {
        [Display(Name = "ProjectStatusActive")]
        Active = 1,

        [Display(Name = "ProjectStatusClosed")]
        Closed = 2
    }
}