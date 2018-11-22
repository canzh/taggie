using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Projectitemeffortqa
    {
        public int Id { get; set; }
        public int ProjectItemId { get; set; }
        public string EffortUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TeamId { get; set; }
        public int ProjectId { get; set; }

        public Projectitem ProjectItem { get; set; }
    }
}
