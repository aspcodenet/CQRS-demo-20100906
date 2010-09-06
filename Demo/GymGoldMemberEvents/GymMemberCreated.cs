using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Eventing;
using ProtoBuf;

namespace GymGoldMemberEvents
{
    [ProtoContract]
    public class GymMemberCreated : IEvent
    {
        public GymMemberCreated()
        {

        }

        public GymMemberCreated(Guid aggid, string sName, string sMemberNo, bool bIsGoldMember, GymGoldMemberEvents.Enums.TrainingLevels level)
        {
            AggregateId = aggid;
            Created = DateTime.Now;
            Name = sName;
            MemberNo = sMemberNo;
            IsGoldMember = bIsGoldMember;
            Level = level;
        }


        [ProtoMember(5)]
        public string Name { get; set; }
        [ProtoMember(6)]
        public string MemberNo { get; set; }
        [ProtoMember(7)]
        public bool IsGoldMember { get; set; }
        [ProtoMember(8)]
        public GymGoldMemberEvents.Enums.TrainingLevels Level { get; set; }


        //IEvent
        [ProtoMember(1)] public Guid AggregateId { get; set; }
        [ProtoMember(2)] public int SequenceNo { get; set; }
        [ProtoMember(3)] public DateTime Created { get; set; }
        [ProtoMember(4)] public int AutoInc { get; set; }
        //



    }
}
