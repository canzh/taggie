using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Api.EFModels.dto
{
    public class TaggieProject
    {
        public int Id { get; set; }

        public string ProjectName { get; set; }

        public string Description { get; set; }
        
        public int AssginedItemsCount { get; set; }
    }
}
