using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Eventing.Dispatcher;

namespace ReadModelUpdater.Handlers
{
    public class GymMemberTrainingRegistered : HandlesEvent<GymGoldMemberEvents.GymMemberTrainingRegistered>
    {
        protected System.Data.SqlClient.SqlConnection conn;
        public GymMemberTrainingRegistered(IDbConnection conn)
        {
            this.conn = conn as System.Data.SqlClient.SqlConnection;
        }


        public override void Handle(GymGoldMemberEvents.GymMemberTrainingRegistered msg)
        {
            //This is something we dont show - as of now that is so we dont care

        }
    }

}
