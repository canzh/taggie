using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Contentfile
    {
        public Contentfile()
        {
            Filecategory = new HashSet<Filecategory>();
            Filesubcategory = new HashSet<Filesubcategory>();
            Projectitemcategories = new HashSet<Projectitemcategories>();
            Projectitemsubcategories = new HashSet<Projectitemsubcategories>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string OriginalUrl { get; set; }
        public int FilePathId { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FinishedBy { get; set; }
        public DateTime? FinishedDate { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }

        public Contentfilepath FilePath { get; set; }
        public ICollection<Filecategory> Filecategory { get; set; }
        public ICollection<Filesubcategory> Filesubcategory { get; set; }
        public ICollection<Projectitemcategories> Projectitemcategories { get; set; }
        public ICollection<Projectitemsubcategories> Projectitemsubcategories { get; set; }
    }
}
