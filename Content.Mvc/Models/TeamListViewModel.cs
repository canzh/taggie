using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Mvc.Models
{
    public class TeamListViewModel
    {
        public int SelectedTeamId { get; set; }

        public int AssignedItemCount { get; set; }

        [Display(Name = "TeamList")]
        public List<SelectListItem> Teams { get; set; }
    }
}
