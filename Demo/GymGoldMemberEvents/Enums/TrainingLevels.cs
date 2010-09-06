using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace GymGoldMemberEvents.Enums
{
    [Flags]
    public enum TrainingLevels
    {
        [ProtoEnum]
        TrainingLevelsUnknown = 0,
        [ProtoEnum]
        Beginner = 10,
        [ProtoEnum]
        Intermediate = 20,
        [ProtoEnum]
        Expert = 30

    }
}
