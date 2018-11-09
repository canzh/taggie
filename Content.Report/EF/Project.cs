using System;
using System.Collections.Generic;

namespace Content.Report.EF
{
    public partial class Project
    {
        public Project()
        {
            Teamprojects = new HashSet<Teamprojects>();
        }

        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string BaseDir { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Teamprojects> Teamprojects { get; set; }
    }
}
