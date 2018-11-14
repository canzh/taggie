using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Projectitemkeywords
    {
        public int Id { get; set; }
        public int ProjectItemId { get; set; }
        public int KeywordId { get; set; }
        public byte AddedByRole { get; set; }

        public Projectkeyword Keyword { get; set; }
        public Projectitem ProjectItem { get; set; }
    }
}
