using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Projectcategory
    {
        public Projectcategory()
        {
            Projectitemcategories = new HashSet<Projectitemcategories>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte Status { get; set; }
        public int ProjectId { get; set; }
        public DateTime CreatedDate { get; set; }

        public Project Project { get; set; }
        public ICollection<Projectitemcategories> Projectitemcategories { get; set; }
    }
}
