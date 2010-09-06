using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Eventing.Dispatcher;

namespace ReadModelUpdater.Handlers
{
    public class GymMemberLostGoldStatus : HandlesEvent<GymGoldMemberEvents.GymMemberLostGoldStatus>
    {
        protected System.Data.SqlClient.SqlConnection conn;
        public GymMemberLostGoldStatus(IDbConnection conn)
        {
            this.conn = conn as System.Data.SqlClient.SqlConnection;
        }


        public override void Handle(GymGoldMemberEvents.GymMemberLostGoldStatus msg)
        {

            DataTable dt =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(conn, CommandType.Text,
                                                                          "select * from report_memberlist where id='" +
                                                                          msg.AggregateId.ToString() + "'").
                    Tables[0];
            if (dt.Rows.Count == 0)
                return;

            string sql = @"update report_memberlist set isgold='N' where id=@id";

            var p = new List<System.Data.SqlClient.SqlParameter>
                        {
                            new System.Data.SqlClient.SqlParameter("@id", msg.AggregateId)
                        };
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, p.ToArray());



        }
    }

}
