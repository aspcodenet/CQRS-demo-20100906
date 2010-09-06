using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Eventing.Dispatcher;

namespace ReadModelUpdater.Handlers
{
    public class GymMemberCreated : HandlesEvent<GymGoldMemberEvents.GymMemberCreated>
    {
        protected System.Data.SqlClient.SqlConnection conn;
        public GymMemberCreated(IDbConnection conn)
        {
            this.conn = conn as System.Data.SqlClient.SqlConnection;
        }


        public override void Handle(GymGoldMemberEvents.GymMemberCreated msg)
        {

            DataTable dt =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(conn, CommandType.Text,
                                                                          "select * from report_memberlist where id='" +
                                                                          msg.AggregateId.ToString() + "'").
                    Tables[0];
            if (dt.Rows.Count > 0)
                return;

            string sql = @"insert report_memberlist(id,membername,traininglevel, isgold, memberno) select  @id,@membername, @traininglevel, @isgold, @memberno";

            var p = new List<System.Data.SqlClient.SqlParameter>
                        {
                            new System.Data.SqlClient.SqlParameter("@id", msg.AggregateId),
                            new System.Data.SqlClient.SqlParameter("@membername", msg.Name),
                            new System.Data.SqlClient.SqlParameter("@traininglevel", msg.Level.ToString()),
                            new System.Data.SqlClient.SqlParameter("@isgold", msg.IsGoldMember ? "J" : "N"),
                            new System.Data.SqlClient.SqlParameter("@memberno", msg.MemberNo   )
                        };
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, p.ToArray());



        }
    }

}
