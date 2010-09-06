using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bus;

namespace GoldGymMemberCommandHandler
{
    public class CorrectTrainingLevel : Handles<GymGoldMemberCommands.DowngradeTrainingLevel>
    {
        private readonly GoldGymMemberDomain.Ports.IGymMemberRepository _mRepMembers;
        public CorrectTrainingLevel(GoldGymMemberDomain.Ports.IGymMemberRepository repMembers)
        {
            _mRepMembers = repMembers;
        }

        public override void SetupPolicies()
        {
            Policy_LogBefore(base.Log, (logg, msg) => logg.Info(msg.GetType().ToString() + " received"));
            Policy_LogAfter(base.Log, (logg, msg) => logg.Info(msg.GetType().ToString() + " done"));
            Policy_Database(isTransactional: false);
        }

        public override void Handle(GymGoldMemberCommands.DowngradeTrainingLevel msg)
        {
            GoldGymMemberDomain.GymMember member = _mRepMembers.GetById(msg.MemberId);
            GymGoldMemberEvents.Enums.TrainingLevels lev;
            Enum.TryParse(msg.NewLevel.ToString(), true, out lev);
            member.CorrectTrainingLevel(lev);
            _mRepMembers.SaveChanges(member.AggregateId, member.GetType().ToString(), member.SequenceNoAR, member.GetChanges());
        }
    }

}
