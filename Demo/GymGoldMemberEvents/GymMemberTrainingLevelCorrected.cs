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
    public class GymMemberTrainingLevelCorrected : IEvent   
    {
        public GymMemberTrainingLevelCorrected()
        {

        }

        public GymMemberTrainingLevelCorrected(Guid aggid, Enums.TrainingLevels level)
        {
            AggregateId = aggid;
            Created = DateTime.Now;
            Level = level;
        }


        [ProtoMember(8), DefaultValue(GymGoldMemberEvents.Enums.TrainingLevels.Beginner)]
        public Enums.TrainingLevels Level { get; set; }


        //IEvent
        [ProtoMember(1)] public Guid AggregateId { get; set; }
        [ProtoMember(2)] public int SequenceNo { get; set; }
        [ProtoMember(3)] public DateTime Created { get; set; }
        [ProtoMember(4)] public int AutoInc { get; set; }
        //
    }
}
