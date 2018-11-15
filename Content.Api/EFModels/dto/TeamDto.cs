using Content.Api.EFModels.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Api.EFModels.dto
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public int MemberCount { get; set; }
        public string Description { get; set; }
        public TeamStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<TeamAssignment> AssignedProjects { get; set; }
    }
}
