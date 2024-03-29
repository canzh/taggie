﻿using Content.Api.EFModels.enums;
using System;
using System.Collections.Generic;

namespace Content.Api.EFModels
{
    public partial class Projectitemsubcategories
    {
        public int Id { get; set; }
        public int ProjectItemId { get; set; }
        public int SubCategoryId { get; set; }
        public UserRoleType AddedByRole { get; set; }

        public Projectitem ProjectItem { get; set; }
        public Projectsubcategory SubCategory { get; set; }
    }
}
