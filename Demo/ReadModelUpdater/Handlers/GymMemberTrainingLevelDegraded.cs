﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Eventing.Dispatcher;

namespace ReadModelUpdater.Handlers
{
    public class GymMemberTrainingLevelDegraded : HandlesEvent<GymGoldMemberEvents.GymMemberTrainingLevelDegraded>
    {
        protected System.Data.SqlClient.SqlConnection conn;
        public GymMemberTrainingLevelDegraded(IDbConnection conn)
        {
            this.conn = conn as System.Data.SqlClient.SqlConnection;
        }


        public override void Handle(GymGoldMemberEvents.GymMemberTrainingLevelDegraded msg)
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
