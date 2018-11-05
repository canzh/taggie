using Content.Api.Models;
using System;
using System.Collections.Generic;

namespace Content.Api.EF
{
    public partial class Contentfile
    {
        public Contentfile()
        {
            Filecategory = new HashSet<Filecategory>();
            Filesubcategory = new HashSet<FileSubcategory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string OriginalUrl { get; set; }
        public int FilePathId { get; set; }
        public SourceStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FinishedBy { get; set; }
        public DateTime? FinishedDate { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }

        public Contentfilepath FilePath { get; set; }
        public ICollection<Filecategory> Filecategory { get; set; }
        public ICollection<FileSubcategory> Filesubcategory { get; set; }
    }
}
