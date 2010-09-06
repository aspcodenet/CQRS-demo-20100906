using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Eventing.Dispatcher;
using log4net;

namespace ReadModelUpdater
{
    public class LogChaser
    {
        public static ILog log = log4net.LogManager.GetLogger("CommandHandler");
        readonly string sProcess;
        readonly EventDispatcher dispatcher;
        readonly Castle.Windsor.IWindsorContainer container;
        readonly SqlConnection connReport;
        public LogChaser(string sProcess, EventDispatcher dispatcher, Castle.Windsor.IWindsorContainer container)
        {
            this.sProcess = sProcess;
            this.dispatcher = dispatcher;
            this.container = container;
            connReport = container.Resolve<System.Data.IDbConnection>("dbconnreportdatabase") as SqlConnection;
            lastprocessed = -1;
        }

        private int lastprocessed;

        public int Chase()
        {
            if (lastprocessed == -1)
                lastprocessed = GetLastProcessed(sProcess);
            int cnt = 0;
            var storagehandler = container.Resolve<Eventing.Store.IEventStore>();
            storagehandler.Open(false);
            foreach (Eventing.IEvent b in storagehandler.GetEventsForChasing(lastprocessed, 500))
            {
                if (connReport.State == ConnectionState.Closed)
                    connReport.Open();
                //Handle events
                log.Info(b.GetType().ToString() + " " + b.AutoInc.ToString());
                dispatcher.Dispatch(b);

                cnt++;
                lastprocessed = b.AutoInc;
                //Update last processed
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(connReport, CommandType.Text, "update report_eventchaserlog set lastprocessed=" + lastprocessed.ToString() + " where chaseprocess='" + sProcess + "'");
            }
            storagehandler.Close(true);

            if (connReport.State != ConnectionState.Closed)
                connReport.Close();


            return cnt;
        }
        public int GetLastProcessed(string sProcess)
        {
            if (connReport.State == ConnectionState.Closed)
                connReport.Open();
            DataTable dtChaserLog = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(connReport, CommandType.Text, "select * from report_eventchaserlog where chaseprocess='" + sProcess + "'").Tables[0];

            int lastprocessed = 0;
            if (dtChaserLog.Rows.Count > 0)
                lastprocessed = Convert.ToInt32(dtChaserLog.Rows[0][1]);
            else
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(connReport, CommandType.Text, "insert report_eventchaserlog select '" + sProcess + "',0");
            connReport.Close();
            return lastprocessed;


        }

    }

}
