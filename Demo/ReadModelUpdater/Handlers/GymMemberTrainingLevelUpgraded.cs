using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Eventing.Dispatcher;

namespace ReadModelUpdater.Handlers
{
    public class GymMemberTrainingLevelUpgraded : HandlesEvent<GymGoldMemberEvents.GymMemberTrainingLevelUpgraded>
    {
        protected System.Data.SqlClient.SqlConnection conn;
        public GymMemberTrainingLevelUpgraded(IDbConnection conn)
        {
            this.conn = conn as System.Data.SqlClient.SqlConnection;
        }


        public override void Handle(GymGoldMemberEvents.GymMemberTrainingLevelUpgraded msg)
        {

            string sql = @"update report_memberlist set traininglevel=@level where id=@id";

            var p = new List<System.Data.SqlClient.SqlParameter>
                        {
                            new System.Data.SqlClient.SqlParameter("@id", msg.AggregateId),
                            new System.Data.SqlClient.SqlParameter("@level", msg.NewLevel.ToString())
                        };
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, p.ToArray());



        }
    }
}
