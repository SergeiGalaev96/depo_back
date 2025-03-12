using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Depository.MassTransitQueue
{
    public interface ValueEntered
    {
        string Value { get; set; }
    }
}
