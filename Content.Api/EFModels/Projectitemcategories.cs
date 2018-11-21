using Content.Api.EFModels.enums;
using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Projectitemcategories
    {
        public int Id { get; set; }
        public int ProjectItemId { get; set; }
        public int CategoryId { get; set; }
        public UserRoleType AddedByRole { get; set; }

        public Projectcategory Category { get; set; }
        public Projectitem ProjectItem { get; set; }
    }
}
