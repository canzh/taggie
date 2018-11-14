using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Contentfilepath
    {
        public Contentfilepath()
        {
            Contentfile = new HashSet<Contentfile>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }
        public int? ParentId { get; set; }

        public ICollection<Contentfile> Contentfile { get; set; }
    }
}
