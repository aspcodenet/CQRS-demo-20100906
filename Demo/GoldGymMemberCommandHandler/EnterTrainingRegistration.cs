using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bus;

namespace GoldGymMemberCommandHandler
{
    public class EnterTrainingRegistration : Handles<GymGoldMemberCommands.EnterTrainingRegistration>
    {
        private readonly GoldGymMemberDomain.Ports.IGymMemberRepository _mRepMembers;
        public EnterTrainingRegistration(GoldGymMemberDomain.Ports.IGymMemberRepository repMembers)
        {
            _mRepMembers = repMembers;
        }

        public override void SetupPolicies()
        {
            Policy_LogBefore(base.Log, (logg, msg) => logg.Info(msg.GetType().ToString() + " received"));
            Policy_LogAfter(base.Log, (logg, msg) => logg.Info(msg.GetType().ToString() + " done"));
            Policy_Database(isTransactional: false);
        }

        public override void Handle(GymGoldMemberCommands.EnterTrainingRegistration msg)
        {
            GoldGymMemberDomain.GymMember member = _mRepMembers.GetById(msg.MemberId);
            member.EnterTrainingRegistration(msg.RegistrationRecordId,msg.When);
            _mRepMembers.SaveChanges(member.AggregateId, member.GetType().ToString(), member.SequenceNoAR, member.GetChanges());
        }
    }


}
