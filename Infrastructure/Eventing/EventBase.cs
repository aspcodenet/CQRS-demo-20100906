using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Eventing.Serializing;
using Eventing.Serializing.Internal;

namespace Eventing
{
    /*
    [Serializable]
    public class EventBase
    {
        public Guid AggregateId { get; set; }
        public Int32 SequenceNo { get; set; }
        public Guid ParentCommandId { get; set; }
        public DateTime Created { get; set; }
        public Int32 AutoInc { get; set; }


        public void WriteDataToBase(SerializationWriter writer)
        {
            writer.Write(AggregateId);
            writer.Write((Int32)SequenceNo);
            writer.Write(ParentCommandId);
            writer.Write(Created);
            writer.Write(AutoInc);
        }

        public void SetDataFromBase(SerializationReader reader)
        {
            AggregateId = reader.ReadGuid();
            SequenceNo = reader.ReadInt32();
            ParentCommandId = reader.ReadGuid();
            Created = reader.ReadDateTime();
            AutoInc = reader.ReadInt32();
        }


        public EventBase(Guid aggid, DateTime dtCreated, int sequenceNo = 0, Guid parentcommand = new Guid())
        {
            AggregateId = aggid;
            SequenceNo = sequenceNo;
            ParentCommandId = parentcommand;
            Created = DateTime.Now;
            AutoInc = 0;
        }

        public void SetAggregateAndSequenceNo(Guid aggid, int sequenceno)
        {
            AggregateId = AggregateId;
            SequenceNo = sequenceno;
        }
        public void SetCommand(Guid commandid)
        {
            ParentCommandId = commandid;
        }




    }*/
}
