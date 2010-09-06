using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Eventing.Dispatcher;

namespace ReadModelUpdater.Handlers
{
    public class GymMemberUpgradedToGoldStatus : HandlesEvent<GymGoldMemberEvents.GymMemberUpgradedToGoldStatus>
    {
        protected System.Data.SqlClient.SqlConnection conn;
        public GymMemberUpgradedToGoldStatus(IDbConnection conn)
        {
            this.conn = conn as System.Data.SqlClient.SqlConnection;
        }


        public override void Handle(GymGoldMemberEvents.GymMemberUpgradedToGoldStatus msg)
        {

            string sql = @"update report_memberlist set isgold='J' where id=@id";

            var p = new List<System.Data.SqlClient.SqlParameter>
                        {
                            new System.Data.SqlClient.SqlParameter("@id", msg.AggregateId)
                        };
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, p.ToArray());



        }
    }

}
