using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Projectitem
    {
        public Projectitem()
        {
            Projectitemcategories = new HashSet<Projectitemcategories>();
            Projectitemkeywords = new HashSet<Projectitemkeywords>();
            Projectitemsubcategories = new HashSet<Projectitemsubcategories>();
        }

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string OriginalUrl { get; set; }
        public string RelativeDir { get; set; }
        public string LocalFileName { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public Project Project { get; set; }
        public ICollection<Projectitemcategories> Projectitemcategories { get; set; }
        public ICollection<Projectitemkeywords> Projectitemkeywords { get; set; }
        public ICollection<Projectitemsubcategories> Projectitemsubcategories { get; set; }
    }
}
