using System.ComponentModel.DataAnnotations;

namespace Content.Mvc.Models
{
    public enum TeamStatus
    {
        [Display(Name = "StatusDisabled")]
        Disabled = 0,

        [Display(Name = "StatusActive")]
        Active = 1,
    }
}