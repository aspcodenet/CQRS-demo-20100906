using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymGoldMemberCommands
{
    [Serializable]
    public class EnterTrainingRegistration
    {
        public EnterTrainingRegistration()
        {

        }
        public EnterTrainingRegistration(Guid memberid, Guid idregistrationrecord, DateTime dtDateTime)
        {
            MemberId = memberid;
            RegistrationRecordId = idregistrationrecord;
            When = dtDateTime;
        }
        public Guid MemberId { get; set; }
        public Guid RegistrationRecordId { get; set; }
        public DateTime When { get; set; }

    }

}
