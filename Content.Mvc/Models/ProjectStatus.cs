using System.ComponentModel.DataAnnotations;

namespace Content.Mvc.Models
{
    public enum ProjectStatus
    {
        [Display(Name = "StatusActive")]
        Active = 1,

        [Display(Name = "StatusClosed")]
        Closed = 2
    }
}