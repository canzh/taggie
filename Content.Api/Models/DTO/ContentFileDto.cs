using Content.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Content.Web.Models
{
    public class ContentFileDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public int FilePathId { get; set; }
        public SourceStatus Status { get; set; }
        public string FinishedBy { get; set; }
        public string VerifiedBy { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Subcategories { get; set; }
        public DateTimeOffset? FinishedDate { get; set; }
        public DateTimeOffset? VerifiedDate { get; set; }
    }
}