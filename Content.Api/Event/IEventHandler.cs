using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Content.Api.Event
{
    interface IEventHandler<T>
    {
        void Handle(T o);
    }
}
