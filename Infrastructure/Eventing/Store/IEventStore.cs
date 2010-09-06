using System;
using System.Collections.Generic;
using Eventing.Snapshot.Interfaces;

namespace Eventing.Store
{
    public interface IEventStore
    {
        //General
        void Open(bool transactional);
        void Close(bool ok);

        //Repository functions
        T GetById<T>(Guid id) where T : AggregateRoot, new();

        void SaveChanges(Guid aggId, string aggregateTypeName, int orginatingVersion,
                         IEnumerable<Eventing.IEvent> events);

        //Reporting
        IEnumerable<Eventing.IEvent> GetEventsForChasing(int startpos = 0, int nMaxToRead = -1);
        //IEnumerable<Internal.EventData> GetEventsForChasing(int startPos = 0, int maxToRead = -1);
        //IEnumerable<Internal.EventData> GetEventsForAggregate(Guid aggId, int startSequenceNo = 0, int maxToRead = -1);
        IEnumerable<Eventing.IEvent> GetEventsForAggregate(Guid aggid, int startsequenceno = 0, int nMaxToRead = -1);

        //Snapshots
        Dictionary<Guid, string> GetAggregatesNeedingSnapshot(int nMax);
        void CreateSnapshot(Guid aggid, int SequenceNo, int MementoVersion, IMemento memento);

    }
}
