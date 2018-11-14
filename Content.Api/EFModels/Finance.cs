using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Finance
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int TotalEffort { get; set; }
        public int SettledEffort { get; set; }
    }
}
