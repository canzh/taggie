using System;
using System.Collections.Generic;

namespace Content.Report.EF
{
    public partial class Effortstatisticuser
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int FinishedCount { get; set; }
        public int IncorrectCount { get; set; }
    }
}
