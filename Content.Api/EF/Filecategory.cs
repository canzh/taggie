﻿using System;
using System.Collections.Generic;

namespace Content.Api.EF
{
    public partial class Filecategory
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public int CategoryId { get; set; }

        public Category Category { get; set; }
        public Contentfile File { get; set; }
    }
}
