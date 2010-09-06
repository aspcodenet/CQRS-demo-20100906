using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eventing
{
    public interface IEvent
    {
        Guid AggregateId { get; set; }
        Int32 SequenceNo { get; set; }
        DateTime Created { get; set; }
        Int32 AutoInc { get; set; }
    }
}
