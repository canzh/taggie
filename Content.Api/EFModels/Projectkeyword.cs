using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Projectkeyword
    {
        public Projectkeyword()
        {
            Projectitemkeywords = new HashSet<Projectitemkeywords>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte Status { get; set; }
        public int ProjectId { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Projectitemkeywords> Projectitemkeywords { get; set; }
    }
}
