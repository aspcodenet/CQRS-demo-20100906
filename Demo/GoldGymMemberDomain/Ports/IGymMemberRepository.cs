using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoldGymMemberDomain.Ports
{
    public  interface IGymMemberRepository
    {
        GymMember GetById(Guid id);
        void SaveChanges(Guid aggid, string sAggregateTypeName, int nOrginatingVersion, IEnumerable<Eventing.IEvent> events);

    }
}
