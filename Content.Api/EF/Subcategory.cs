﻿using Content.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Content.Api.EF
{
    public partial class Subcategory
    {
        public Subcategory()
        {
            Filesubcategory = new HashSet<FileSubcategory>();
        }

        // nullable for client post request to create new record, Id is generated by DB
        public int? Id { get; set; }
        public string Name { get; set; }
        public CategoryStatus Status { get; set; }

        [JsonIgnore]
        public ICollection<FileSubcategory> Filesubcategory { get; set; }
    }
}
