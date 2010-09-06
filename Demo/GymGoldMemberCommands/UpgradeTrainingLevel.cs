using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymGoldMemberCommands
{
    [Serializable]
    public class UpgradeTrainingLevel
    {
        public UpgradeTrainingLevel()
        {

        }
        public UpgradeTrainingLevel(Guid memberid, Enums.TrainngLevels level)
        {
            MemberId = memberid;
            NewLevel = level;
        }
        public Guid MemberId { get; set; }
        public Enums.TrainngLevels NewLevel { get; set; }

    }

}
