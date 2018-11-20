using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Api.EFModels.dto
{
    public class QueueItemDto
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public int TotalProjectItems { get; set; }

        public int RemainingProjectItems { get; set; }

        public int TaggieFinishedItems { get; set; }

        public int ProjectItemId { get; set; }
    }
}
