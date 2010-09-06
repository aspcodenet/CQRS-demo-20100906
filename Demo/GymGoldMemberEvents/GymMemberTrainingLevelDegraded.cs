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
    public class GymMemberTrainingLevelDegraded : IEvent
    {
        public GymMemberTrainingLevelDegraded()
        {

        }

        public GymMemberTrainingLevelDegraded(Guid aggid, Enums.TrainingLevels level)
        {
            AggregateId = aggid;
            Created = DateTime.Now;
            NewLevel = level;
        }


        [ProtoMember(8), DefaultValue(GymGoldMemberEvents.Enums.TrainingLevels.Beginner)]
        public Enums.TrainingLevels NewLevel { get; set; }


        //IEvent
        [ProtoMember(1)]
        public Guid AggregateId { get; set; }
        [ProtoMember(2)]
        public int SequenceNo { get; set; }
        [ProtoMember(3)]
        public DateTime Created { get; set; }
        [ProtoMember(4)]
        public int AutoInc { get; set; }
        //


    }

}
