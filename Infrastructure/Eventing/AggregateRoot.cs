using System;
using System.Collections.Generic;

namespace Eventing
{
    public class AggregateRoot
    {
        public Guid AggregateId { get; set; }
        public int SequenceNoEvent { get; set; }
        public int SequenceNoAR { get; set; }

        private readonly Dictionary<Type, Action<Eventing.IEvent>> _registeredEvents;
        private readonly List<Eventing.IEvent> _appliedEvents;


        public AggregateRoot()
        {
            _registeredEvents = new Dictionary<Type, Action<Eventing.IEvent>>();
            _appliedEvents = new List<Eventing.IEvent>();
            SequenceNoEvent = 0;
        }


        public void RegisterEventHandler<T>(Action<T> callback) where T : class, Eventing.IEvent
        {
            _registeredEvents.Add(typeof(T), theEvent => callback(theEvent as T));
        }


        protected void ApplyEvent<TEvent>(TEvent domainEvent) where TEvent : Eventing.IEvent
        {
            
            domainEvent.AggregateId =AggregateId;
            domainEvent.SequenceNo =  GetNewEventVersion();
            applyEvent(domainEvent.GetType(), domainEvent);
            _appliedEvents.Add(domainEvent);
        }

        private int GetNewEventVersion()
        {
            return ++SequenceNoEvent;
        }


        public void applyEvent(Type eventType, Eventing.IEvent domainEvent)
        {
            Action<Eventing.IEvent> handler;

            if (!_registeredEvents.TryGetValue(eventType, out handler))
                return;

            handler(domainEvent);
        }



        public IEnumerable<Eventing.IEvent> GetChanges()
        {
            return _appliedEvents;
        }



        public AggregateRoot(Guid aggid, int sequenceNo = 0)
        {
            AggregateId = aggid;
            SequenceNoAR = sequenceNo;
        }


    }
}
