using Content.Api.EFModels.enums;
using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Projectkeyword
    {
        public Projectkeyword()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public TagStatus Status { get; set; }
        public int ProjectId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
