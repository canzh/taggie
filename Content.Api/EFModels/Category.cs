using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Category
    {
        public Category()
        {
            Filecategory = new HashSet<Filecategory>();
            Projectitemcategories = new HashSet<Projectitemcategories>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte Status { get; set; }

        public ICollection<Filecategory> Filecategory { get; set; }
        public ICollection<Projectitemcategories> Projectitemcategories { get; set; }
    }
}
