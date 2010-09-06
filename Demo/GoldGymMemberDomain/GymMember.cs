using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoldGymMemberDomain.Enums;
using GymGoldMemberEvents;

namespace GoldGymMemberDomain
{
    public class GymMember : Eventing.AggregateRoot
    {
        private List<TrainingRegistration> _mTrainingregistrations;

        public string Name { get; private set; }
        public string MemberNo { get; private set; }
        public bool IsGoldMember { get; private set; }
        public GoldGymMemberDomain.Enums.TrainngLevels CurrentTrainingLevel { get; private set; }

        //BEHAVIOUR

        public static GymMember CreateNew(Guid guid, string sName)
        {
            return new GymMember(guid, sName);
        }
        
        public void EnterTrainingRegistration(Guid idregistrationrecord, DateTime dtDateTime )
        {
            //Check for duplicate
            if (_mTrainingregistrations.Count(p => p.Id == idregistrationrecord) > 0)
                return;

            var oEvent = new GymMemberTrainingRegistered(AggregateId, idregistrationrecord , dtDateTime );
            ApplyEvent<GymMemberTrainingRegistered>(oEvent);

            //Check for gold member status
            CheckGoldMemberStatus();

        }

        public void CheckGoldMemberStatus()
        {
            if (IsGoldMember == true)
                checkForDegrade();
            else
            {
                checkForUpgrade();
            }
        }

        public void UpgradeTrainingLevel(GymGoldMemberEvents.Enums.TrainingLevels newlevel)
        {
            GoldGymMemberDomain.Enums.TrainngLevels lev;
            Enum.TryParse(newlevel.ToString(), true, out lev);
            if (CurrentTrainingLevel != lev && CurrentTrainingLevel < lev)
            {
                ApplyEvent<GymMemberTrainingLevelUpgraded>(new GymMemberTrainingLevelUpgraded(AggregateId, newlevel));
            }

        }

        public void DegradeTrainingLevel(GymGoldMemberEvents.Enums.TrainingLevels newlevel)
        {
            GoldGymMemberDomain.Enums.TrainngLevels lev;
            Enum.TryParse(newlevel.ToString(), true, out lev);
            if (CurrentTrainingLevel != lev && CurrentTrainingLevel < lev)
            {
                ApplyEvent<GymMemberTrainingLevelDegraded>(new GymMemberTrainingLevelDegraded(AggregateId, newlevel));
            }

        }


        public void CorrectTrainingLevel(GymGoldMemberEvents.Enums.TrainingLevels level)
        {
            GoldGymMemberDomain.Enums.TrainngLevels lev;
            Enum.TryParse(level.ToString(), true, out lev);
            if (CurrentTrainingLevel != lev)
            {
                ApplyEvent<GymMemberTrainingLevelCorrected>(new GymMemberTrainingLevelCorrected(AggregateId, level));
            }

        }

        private void checkForUpgrade()
        {
            //When trained > 12 times last 30 days
            DateTime dt30DaysBack = DateTime.Now.AddDays(-30);
            if ( _mTrainingregistrations.Count(p=>p.When > dt30DaysBack) > 12 )
            {
                //upgrade
                ApplyEvent(new GymMemberUpgradedToGoldStatus(AggregateId,DateTime.Now ));
            }

        }

        private void checkForDegrade()
        {
            DateTime dt60DaysBack = DateTime.Now.AddDays(-60);
            if (_mTrainingregistrations.Count(p => p.When > dt60DaysBack) > 10)
            {
                //upgrade
                ApplyEvent(new GymMemberLostGoldStatus(AggregateId, DateTime.Now));
            }
        }


        private string GenerateMemberNo()
        {
            return DateTime.Now.ToString("yyMMddHHmmss");
        }

        public GymMember(Guid guid, string sNamn)
            : this()
        {
            AggregateId = guid;

            var oEvent = new GymMemberCreated(guid, sNamn, GenerateMemberNo(), false, GymGoldMemberEvents.Enums.TrainingLevels.Beginner);
            ApplyEvent<GymMemberCreated>(oEvent);

        }
        public GymMember()
        {
            _mTrainingregistrations = new List<TrainingRegistration>();
            RegisterEventHandler<GymMemberCreated>(onGymMemberCreated);
            RegisterEventHandler<GymMemberTrainingRegistered>(onGymMemberTrainingRegistered);
            RegisterEventHandler<GymMemberUpgradedToGoldStatus>(onGymMemberUpgradedToGoldStatus);
            RegisterEventHandler<GymMemberTrainingLevelCorrected>(onGymMemberTrainingLevelCorrected);
            RegisterEventHandler<GymMemberTrainingLevelDegraded>(onGymMemberTrainingLevelDegraded);
            RegisterEventHandler<GymMemberTrainingLevelUpgraded>(onGymMemberTrainingLevelUpgraded);
            
            
        }

        private void onGymMemberTrainingLevelUpgraded(GymMemberTrainingLevelUpgraded obj)
        {
            GoldGymMemberDomain.Enums.TrainngLevels lev;
            Enum.TryParse(obj.NewLevel.ToString(), true, out lev);
            CurrentTrainingLevel = lev;
        }

        private void onGymMemberTrainingLevelDegraded(GymMemberTrainingLevelDegraded obj)
        {
            GoldGymMemberDomain.Enums.TrainngLevels lev;
            Enum.TryParse(obj.NewLevel.ToString(), true, out lev);
            CurrentTrainingLevel = lev;
        }

        private void onGymMemberTrainingLevelCorrected(GymMemberTrainingLevelCorrected obj)
        {
            GoldGymMemberDomain.Enums.TrainngLevels lev;
            Enum.TryParse(obj.Level.ToString(), true, out lev);
            CurrentTrainingLevel = lev;
        }

        private void onGymMemberUpgradedToGoldStatus(GymMemberUpgradedToGoldStatus obj)
        {
            this.IsGoldMember = true;
        }

        private void onGymMemberTrainingRegistered(GymMemberTrainingRegistered obj)
        {
            if (_mTrainingregistrations.Count(p => p.Id == obj.RegistrationRecordId) > 0)
                return;
            _mTrainingregistrations.Add(new TrainingRegistration { Id = obj.RegistrationRecordId, When = obj.When });
        }

        private void onGymMemberCreated(GymMemberCreated obj)
        {
            Name = obj.Name;
            MemberNo = obj.MemberNo;
            IsGoldMember = obj.IsGoldMember;
            GoldGymMemberDomain.Enums.TrainngLevels lev;
            Enum.TryParse(obj.Level.ToString(), true, out lev);
            CurrentTrainingLevel = lev;

        }
    }
}
