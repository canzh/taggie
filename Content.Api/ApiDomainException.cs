using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Api
{
    public class ApiDomainException : Exception
    {
        public ApiDomainException()
        { }

        public ApiDomainException(string message)
            : base(message)
        { }

        public ApiDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
