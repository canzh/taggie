using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Api.Event
{
    public class TaggedEvent
    {
        public int TeamId { get; set; }
        public int ProjectId { get; set; }
        public int ProjectItemId { get; set; }
        public string TaggedUserId { get; set; }
        public int[] CategoryIds { get; set; }
        public int[] SubcategoryIds { get; set; }
        public string[] Keywords { get; set; }
    }
}
