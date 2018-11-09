using System;
using System.Collections.Generic;

namespace Content.Report.EF
{
    public partial class Team
    {
        public Team()
        {
            Teamprojects = new HashSet<Teamprojects>();
        }

        public int Id { get; set; }
        public string TeamName { get; set; }
        public int UsersCount { get; set; }
        public string Description { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Teamprojects> Teamprojects { get; set; }
    }
}
