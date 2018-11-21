using Content.Api.EFModels.enums;
using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Projectsubcategory
    {
        public Projectsubcategory()
        {
            Projectitemsubcategories = new HashSet<Projectitemsubcategories>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public TagStatus Status { get; set; }
        public int ProjectId { get; set; }
        public DateTime CreatedDate { get; set; }

        public Project Project { get; set; }
        public ICollection<Projectitemsubcategories> Projectitemsubcategories { get; set; }
    }
}
