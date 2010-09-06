using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymGoldMemberCommands
{
    [Serializable]
    public class CheckGoldMemberStatus
    {
        public CheckGoldMemberStatus()
        {

        }
        public CheckGoldMemberStatus(Guid memberid)
        {
            MemberId = memberid;
        }
        public Guid MemberId { get; set; }

    }
}
