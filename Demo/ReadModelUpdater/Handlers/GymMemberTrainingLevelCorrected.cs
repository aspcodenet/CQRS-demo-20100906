using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Eventing.Dispatcher;

namespace ReadModelUpdater.Handlers
{
    public class GymMemberTrainingLevelCorrected : HandlesEvent<GymGoldMemberEvents.GymMemberTrainingLevelCorrected>
    {
        protected System.Data.SqlClient.SqlConnection conn;
        public GymMemberTrainingLevelCorrected(IDbConnection conn)
        {
            this.conn = conn as System.Data.SqlClient.SqlConnection;
        }


        public override void Handle(GymGoldMemberEvents.GymMemberTrainingLevelCorrected msg)
        {

            string sql = @"update report_memberlist set traininglevel=@level where id=@id";

            var p = new List<System.Data.SqlClient.SqlParameter>
                        {
                            new System.Data.SqlClient.SqlParameter("@id", msg.AggregateId),
                            new System.Data.SqlClient.SqlParameter("@level", msg.Level.ToString())
                        };
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, p.ToArray());



        }
    }

}
