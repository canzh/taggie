using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Mvc.Models
{
    public class TaggieProjectListViewModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
        
        [Display(Name = "AssginedItemsCount")]
        public int AssginedItemsCount { get; set; }
    }
}
