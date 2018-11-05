using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Content.Api.Models
{
    public class ContentFilePathDto
    {
        public ContentFilePathDto()
        {
            children = new List<ContentFilePathDto>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public bool leaf { get; set; }
        public bool loaded { get; set; }
        public bool expanded { get; set; }

        public List<ContentFilePathDto> children { get; set; }
    }

    public class ContentFilePathTreeModel
    {
        public bool success { get; set; }
        public ContentFilePathDto children { get; set; }
    }

}