using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Financehistory
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public byte FinanceType { get; set; }
        public int FinanceNumber { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
