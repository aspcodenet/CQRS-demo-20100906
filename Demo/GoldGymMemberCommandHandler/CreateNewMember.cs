using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bus;

namespace GoldGymMemberCommandHandler
{
    public class CreateNewMember : Handles<GymGoldMemberCommands.CreateNewMember>
    {
        private readonly GoldGymMemberDomain.Ports.IGymMemberRepository _mRepMembers;
        public CreateNewMember(GoldGymMemberDomain.Ports.IGymMemberRepository repMembers)
        {
            _mRepMembers = repMembers;
        }

        public override void SetupPolicies()
        {
            Policy_LogBefore(base.Log, (logg, msg) => logg.Info(msg.GetType().ToString() + " received"));
            Policy_LogAfter(base.Log, (logg, msg) => logg.Info(msg.GetType().ToString() + " done"));
            Policy_Database(isTransactional: false);
        }

        public override void Handle(GymGoldMemberCommands.CreateNewMember msg)
        {
            GoldGymMemberDomain.GymMember member = GoldGymMemberDomain.GymMember.CreateNew(msg.MemberId, msg.Name);
            _mRepMembers.SaveChanges(member.AggregateId, member.GetType().ToString(), member.SequenceNoAR, member.GetChanges());
        }
    }

}
