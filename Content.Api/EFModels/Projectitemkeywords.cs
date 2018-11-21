using Content.Api.EFModels.enums;
using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Projectitemkeywords
    {
        public int Id { get; set; }
        public int ProjectItemId { get; set; }
        public string Keywords { get; set; }
        public UserRoleType AddedByRole { get; set; }

        public Projectitem ProjectItem { get; set; }
    }
}
