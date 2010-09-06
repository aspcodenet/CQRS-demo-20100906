using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymGoldMemberCommands
{
    [Serializable]
    public class CreateNewMember
    {
        public CreateNewMember()
        {
            
        }
        public CreateNewMember(Guid memberid, string name)
        {
            MemberId = memberid;
            Name = name;
        }
        public Guid MemberId { get; set; }
        public string Name { get; set; }

    }
}
