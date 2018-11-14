﻿using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Teamprojects
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int ProjectId { get; set; }
        public int AssignedProjectItems { get; set; }

        public Project Project { get; set; }
        public Team Team { get; set; }
    }
}
