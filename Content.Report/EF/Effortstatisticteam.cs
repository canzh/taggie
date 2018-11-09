using System;
using System.Collections.Generic;

namespace Content.Report.EF
{
    public partial class Effortstatisticteam
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int FinishedCount { get; set; }
        public int IncorrectCount { get; set; }
    }
}
