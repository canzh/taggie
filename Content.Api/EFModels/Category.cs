using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Category
    {
        public Category()
        {
            Filecategory = new HashSet<Filecategory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte Status { get; set; }

        public ICollection<Filecategory> Filecategory { get; set; }
    }
}
