using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Mvc.Models
{
    public class TaggieQueueSubmitModel
    {
        public int ProjectItemId { get; set; }
        public string[] CategoryNames { get; set; }
        public string[] SubcategoryNames { get; set; }
        public string[] Keywords { get; set; }
    }
}
