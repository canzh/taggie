using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Projectitemsubcategories
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public int SubCategoryId { get; set; }
        public byte Type { get; set; }

        public Contentfile File { get; set; }
        public Subcategory SubCategory { get; set; }
    }
}
