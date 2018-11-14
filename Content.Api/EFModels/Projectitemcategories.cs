using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Projectitemcategories
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public int CategoryId { get; set; }
        public byte Type { get; set; }

        public Category Category { get; set; }
        public Contentfile File { get; set; }
    }
}
