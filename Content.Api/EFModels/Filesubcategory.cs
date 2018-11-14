using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Filesubcategory
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public int SubCategoryId { get; set; }

        public Contentfile File { get; set; }
        public Subcategory SubCategory { get; set; }
    }
}
