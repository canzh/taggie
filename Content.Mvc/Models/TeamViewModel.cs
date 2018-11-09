using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Mvc.Models
{
    public class TeamViewModel
    {
        public int Id { get; set; }
        [Display(Name = "TeamName")]
        public string TeamName { get; set; }
        [Display(Name = "MemberCount")]
        public int MemberCount { get; set; }
        [Display(Name = "Status")]
        public TeamStatus Status { get; set; }
        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "IsQATeam")]
        public bool IsQATeam { get; set; }
    }
}
