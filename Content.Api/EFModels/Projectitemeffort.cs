using Content.Api.EFModels.enums;
using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Projectitemeffort
    {
        public int Id { get; set; }
        public int ProjectItemId { get; set; }
        public string EffortUserId { get; set; }
        public UserRoleType EffortUserRole { get; set; }
        public DateTime CreatedDate { get; set; }

        public Projectitem ProjectItem { get; set; }
    }
}
