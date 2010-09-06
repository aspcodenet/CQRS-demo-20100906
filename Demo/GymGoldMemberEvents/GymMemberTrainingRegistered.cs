﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eventing;
using ProtoBuf;

namespace GymGoldMemberEvents
{
    [ProtoContract]
    public class GymMemberTrainingRegistered : IEvent   
    {
        public GymMemberTrainingRegistered()
        {

        }

        public GymMemberTrainingRegistered(Guid aggid, Guid idRegistrationRecord, DateTime dtWhen )
        {
            AggregateId = aggid;
            Created = DateTime.Now;
            RegistrationRecordId = idRegistrationRecord;
            When = dtWhen;
        }


        [ProtoMember(5)]
        public Guid RegistrationRecordId { get; set; }
        [ProtoMember(6)]
        public DateTime When { get; set; }


        //IEvent
        [ProtoMember(1)] public Guid AggregateId { get; set; }
        [ProtoMember(2)] public int SequenceNo { get; set; }
        [ProtoMember(3)] public DateTime Created { get; set; }
        [ProtoMember(4)] public int AutoInc { get; set; }
        //


    }
}
