using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Effortstatisticqa
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string QauserId { get; set; }
        public string QauserName { get; set; }
        public int EffortCount { get; set; }
    }
}
