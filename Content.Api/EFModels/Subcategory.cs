using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Subcategory
    {
        public Subcategory()
        {
            Filesubcategory = new HashSet<Filesubcategory>();
            Projectitemsubcategories = new HashSet<Projectitemsubcategories>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte Status { get; set; }

        public ICollection<Filesubcategory> Filesubcategory { get; set; }
        public ICollection<Projectitemsubcategories> Projectitemsubcategories { get; set; }
    }
}
