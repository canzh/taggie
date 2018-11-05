using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Content.Api.Models
{
    public enum SourceStatus
    {
        New = 1,
        Tagged,
        Assigned,
        Verified,
        Archived
    }
}