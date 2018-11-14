using Content.Api.EFModels.enums;
using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Team
    {
        public Team()
        {
            Teamprojects = new HashSet<Teamprojects>();
        }

        public int Id { get; set; }
        public string TeamName { get; set; }
        public int MemberCount { get; set; }
        public string Description { get; set; }
        public TeamStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Teamprojects> Teamprojects { get; set; }
    }
}
