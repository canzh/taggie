using Content.Mvc.Services.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Mvc.Models
{
    public class TeamViewModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string TeamName { get; set; }

        [Display(Name = "Description")]
        [StringLength(1000)]
        public string Description { get; set; }

        [Display(Name = "MemberCount")]
        public int MemberCount { get; set; }

        [Display(Name = "Status")]
        public TeamStatus Status { get; set; }

        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "IsQATeam")]
        public bool IsQATeam { get; set; }

        [Display(Name = "TeamMembers")]
        public List<IdentityResponseModel> TeamMembers { get; set; }

        [Display(Name = "AssignedProject")]
        public List<TeamAssignmentViewModel> AssignedProjects { get; set; }
    }
}
