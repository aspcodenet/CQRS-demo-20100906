using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Eventing;
using Eventing.Serializing;
using Eventing.Snapshot.Interfaces;
using Eventing.Store;

namespace EventStoreSQL
{
    public class EventStore : global::Eventing.Store.IEventStore
    {
        private readonly System.Data.IDbConnection _conn;
        public EventStore(System.Data.IDbConnection conn)
        {
            this._conn = conn;
        }

        private static void AddParameter(IDbCommand command, string parameterName, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;

            if (parameter.Value == DBNull.Value)
                parameter.DbType = DbType.Binary;
            command.Parameters.Add(parameter);

        }


        public IEnumerable<Eventing.IEvent> GetEventsForChasing(int startPos, int maxToRead)
        {
            var ret = new List<Eventing.IEvent>();

            System.Data.IDbCommand cmd = _conn.CreateCommand();
            string sql = "select ";
            if (maxToRead != -1)
                sql += " top " + maxToRead.ToString();
            sql += " * from eventstore where AutoInc>" + startPos.ToString() + " order by AutoInc";
            cmd.CommandText = sql;

            IDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                var ev = EventStoreSerializer.Instance.Deserialize(GetBlob(r, 3)) as IEvent;
                ev.AutoInc = r.GetInt32(4);
                ret.Add(ev);
            }
            r.Close();

            return ret;
        }


#if(false)
        public IEnumerable<Eventing.IEvent> GetEventsForChasing(int startPos, int maxToRead)
        {
            var ret = new List<Eventing.IEvent>();

            //Locking problems caused by select * from autoinc...
            //trying twostep = first just ids
            //never the last one though

            //Test 2
            //läs max autoinc
            //Ta 


            int first = -1;
            int last = -1;

            System.Data.IDbCommand cmd = _conn.CreateCommand();
            string sql = "select ";
            if (maxToRead != -1)
                sql += " top " + maxToRead.ToString();
            sql += " autoinc from eventstore where AutoInc>" + startPos.ToString() + " order by AutoInc";
            cmd.CommandText = sql;

            IDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                if (first == -1)
                    first = Convert.ToInt32(r[0]);
                last = Convert.ToInt32(r[0]);
            }
            r.Close();


            if (first == -1 || last == -1 || last-first < 2)
                return ret;

            --last;
            System.Data.IDbCommand cmd = _conn.CreateCommand();
            string sql = "select ";
            if (maxToRead != -1)
                sql += " top " + maxToRead.ToString();
            sql += " * from eventstore where AutoInc>" + startPos.ToString() + " order by AutoInc";
            cmd.CommandText = sql;
            IDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                var ev = EventStoreSerializer.Instance.Deserialize(GetBlob(r, 3)) as IEvent;
                ev.AutoInc = r.GetInt32(4);
                ret.Add(ev);
            }
            r.Close();

            return ret;
        }
#endif


        public static byte[]GetBlob(IDataReader r, int colindex)
        {
            long bloblen = r.GetBytes(colindex, 0, null, 0, 0);
            byte[] b = new byte[bloblen];
            r.GetBytes(colindex, 0, b, 0, b.Length);
            return b;
        }


        public IEnumerable<Eventing.IEvent> GetEventsForAggregate(Guid aggId, int startSequenceNo, int maxToRead)
        {
            var ret = new List<Eventing.IEvent>();

            System.Data.IDbCommand cmd = _conn.CreateCommand();
            string sql = "select ";
            if (maxToRead != -1)
                sql += " top " + maxToRead.ToString();
            sql += " * from eventstore where aggregateid=@aggid and  sequenceno>" + startSequenceNo.ToString() + " order by startsequenceno";
            cmd.CommandText = sql;
            AddParameter(cmd, "@aggid", aggId);


            IDataReader r = null;
            try
            {
                r = cmd.ExecuteReader();
                while (r.Read())
                {
                    var ev = EventStoreSerializer.Instance.Deserialize(GetBlob(r, 3)) as IEvent;
                    if (ev != null)
                    {
                        ev.AutoInc = r.GetInt32(4);
                        ret.Add(ev);
                    }
                    else
                    {
                        throw new SerializationException("Could not load events for " + aggId.ToString());
                    }
                }

            }
            finally 
            {
                if ( r != null)
                    r.Close();
            }

            return ret;

        }

        public Dictionary<Guid, string> GetAggregatesNeedingSnapshot(int nMax)
        {
            var ret = new Dictionary<Guid, string>();
            var cmd = _conn.CreateCommand();
            cmd.CommandText = @"select top " + nMax.ToString() + @" aggregatestore.aggregateid, aggregatestore.aggregatetypename from aggregatestore
left outer join snapshotstore
on aggregatestore.aggregateid=snapshotstore.aggregateid
where 
aggregatestore.latestsequenceno - (case when snapshotstore.aggregateid is null then 0 else snapshotstore.sequenceno end) > 20";

            var r = cmd.ExecuteReader();
            while (r.Read())
            {
                ret.Add(r.GetGuid(0), r.GetString(1));
            }
            r.Close();
            return ret;

        }

        public void CreateSnapshot(Guid aggid, int SequenceNo, int MementoVersion, IMemento memento)
        {
            string scmd = "update snapshotstore set sequenceno=" + SequenceNo.ToString();
            scmd += ",mementoversion=" + MementoVersion.ToString() + ",serdata=@data where aggregateid=@aggid" + Environment.NewLine;
            scmd += " if @@rowcount = 0 begin" + Environment.NewLine;
            scmd += " insert snapshotstore(aggregateid,sequenceno,mementoversion,serdata) values(@aggid," + SequenceNo.ToString() + "," + MementoVersion.ToString() + ",@data)  " + Environment.NewLine;
            scmd += " end";

            var cmd = _conn.CreateCommand();
            cmd.CommandText = scmd;

            AddParameter(cmd, "@aggid", aggid);
            AddParameter(cmd, "@data", Eventing.Serializing.EventStoreSerializer.Instance.Serialize(memento));
            cmd.ExecuteNonQuery();
        }

        private IDbTransaction _tran;
        public void Open(bool transactional)
        {
            if (_conn.State == System.Data.ConnectionState.Closed)
                _conn.Open();
            if (transactional)
                _tran = _conn.BeginTransaction();

            
        }

        public void Close(bool ok)
        {
            if (_tran != null)
                if (ok)
                    _tran.Commit();
                else
                    _tran.Rollback();
            _conn.Close();
        }

        public T GetById<T>(Guid id) where T : Eventing.AggregateRoot, new()
        {

            T retObj = new T();
            retObj.AggregateId = id;

            //Is there any snapshot?
            int lastsequenceno = -1;
            var memo = retObj as IOrginator;
            bool ok = false;
            if (memo != null)
            {
                System.Data.IDbCommand cmd2 = _conn.CreateCommand();
                cmd2.CommandText = "select * from snapshotstore where aggregateid=@aggid";
                AddParameter(cmd2, "@aggid", id);
                IDataReader r2 = cmd2.ExecuteReader();
                while (r2.Read())
                {
                    object o1 = EventStoreSerializer.Instance.Deserialize(GetBlob(r2, 3));
                    var memento = o1 as IMemento;
                    lastsequenceno = r2.GetInt32(1);
                    memo.SetMemento(memento);
                    retObj.AggregateId = id;
                    retObj.SequenceNoAR = lastsequenceno;
                    retObj.SequenceNoEvent = lastsequenceno;

                    /*        public Guid AggregateId { get; set; }
                            public int SequenceNoEvent { get; set; }
                            public int SequenceNoAR { get; set; }
                                        */
                    ok = true;
                }
                r2.Close();
            }




            System.Data.IDbCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "select * from eventstore where aggregateid=@aggid and sequenceno>" + lastsequenceno.ToString() + " order by SequenceNo";
            AddParameter(cmd, "@aggid", id);

            IDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                var ev = EventStoreSerializer.Instance.Deserialize(GetBlob(r, 3)) as IEvent;
                ev.AutoInc = r.GetInt32(4);

                retObj.applyEvent(ev.GetType(), ev);
                retObj.SequenceNoAR = ev.SequenceNo;
                retObj.SequenceNoEvent = ev.SequenceNo;
                ok = true;
            }
            r.Close();
            return ok ? retObj : null;
        }

        public void SaveChanges(Guid aggId, string aggregateTypeName, int orginatingVersion, IEnumerable<Eventing.IEvent> events)
        {

            var oBuilder = new StringBuilder();

            oBuilder.AppendLine("/*begin tran*/");
            oBuilder.AppendLine(@"
declare @latestver as int
select @latestver=latestsequenceno from aggregatestore where aggregateid=@guid
If @latestver Is null
BEGIN
	select @latestver = 0
	insert into aggregatestore select @guid,@latestver,@aggregatetypename
END
If @latestver <> " + orginatingVersion.ToString() + @"
BEGIN
/*    rollback tran */
	RAISERROR('Concurrency Error',16,1)
	return
END
/* insert events with @latestver+1 */

#EVENTS#

update aggregatestore set LatestSequenceNo=@latestver+" + events.Count().ToString() + @" where AggregateId=@guid
/*commit tran*/
");


            var eventsql = new StringBuilder();
            int num = 1;
            foreach (Eventing.IEvent ev in events)
            {
                eventsql.AppendLine("insert into eventstore(AggregateId,SequenceNo,EventTypeName,serdata) select @guid ,@latestver+" + num.ToString() + ",@type" + num.ToString() + ",@data" + num.ToString()  );
                num++;
            }
            oBuilder.Replace("#EVENTS#", eventsql.ToString());
            //oBuilder.Replace("#EVENTS#", "");


            System.Data.IDbCommand cmd = _conn.CreateCommand();
            cmd.CommandText = oBuilder.ToString();
            AddParameter(cmd, "@guid", aggId);
            AddParameter(cmd, "@aggregatetypename", aggregateTypeName);
            //AddParameter(cmd, "@orginatingsequenceno", OrginatingVersion);

            num = 1;
            foreach (Eventing.IEvent ev in events)
            {
                //string serializeddata = EventStoreInfrastructure.Serialization.SerializerService.Serialize(serializer, ev);
                AddParameter(cmd, "@serializer" + num.ToString(), "protobuf");
                AddParameter(cmd, "@data" + num.ToString(), EventStoreSerializer.Instance.Serialize(ev));
                AddParameter(cmd, "@type" + num.ToString(), ev.GetType().ToString());
                num++;
            }

            cmd.ExecuteNonQuery();
        }
    }
}
