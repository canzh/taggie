using Content.Api.EFModels.enums;
using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Project
    {
        public Project()
        {
            Projectitem = new HashSet<Projectitem>();
            Teamprojects = new HashSet<Teamprojects>();
        }

        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string BaseDir { get; set; }
        public int TotalItems { get; set; }
        public ProjectStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Projectitem> Projectitem { get; set; }
        public ICollection<Teamprojects> Teamprojects { get; set; }
    }
}
