using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Mvc.Models
{
    public class TaggieQueueViewModel
    {

        public int ProjectId { get; set; }

        [Display(Name = "ProjectName")]
        public string ProjectName { get; set; }

        [Display(Name = "TotalProjectItems")]
        public int TotalProjectItems { get; set; }

        [Display(Name = "RemainingProjectItems")]
        public int RemainingProjectItems { get; set; }

        [Display(Name = "FinishedItems")]
        public int TaggieFinishedItems { get; set; }

        public string ProjectDescription { get; set; }

        public int ProjectItemId { get; set; }

        [Display(Name = "Categories")]
        public string SelectedCategories { get; set; }

        [Display(Name = "Subcategories")]
        public string SelectedSubcategories { get; set; }

        [Display(Name = "Keywords")]
        public string AddedKeywords { get; set; }

        public List<string> AllCategories { get; set; }

        public List<string> AllSubcategories { get; set; }
    }
}
