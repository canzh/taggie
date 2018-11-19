using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Api.EFModels.enums
{
    public enum ProjectItemStatus
    {
        New = 1,
        Tagged = 2,
        QAVerified = 3,
        Disable = 4,
        Source404 = 5
    }
}
