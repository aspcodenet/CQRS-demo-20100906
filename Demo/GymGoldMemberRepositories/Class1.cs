using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eventing;
using Eventing.Store;

namespace GymGoldMemberRepositories
{
        public class GymMemberRepository : GoldGymMemberDomain.Ports.IGymMemberRepository
        {
            private readonly IEventStore _eventstore;
            public GymMemberRepository(IEventStore eventstore)
            {
                this._eventstore = eventstore;
            }


            public GoldGymMemberDomain.GymMember GetById(Guid id)
            {
                return _eventstore.GetById<GoldGymMemberDomain.GymMember>(id);
            }

            public void SaveChanges(Guid aggid, string sAggregateTypeName, int nOrginatingVersion, IEnumerable<IEvent> events)
            {
                if (events.Count() == 0)
                    return;
                _eventstore.SaveChanges(aggid, sAggregateTypeName, nOrginatingVersion, events);
            }
        }

}
