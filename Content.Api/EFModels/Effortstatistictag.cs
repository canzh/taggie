using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Effortstatistictag
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int FinishedCount { get; set; }
        public int IncorrectCount { get; set; }
    }
}
