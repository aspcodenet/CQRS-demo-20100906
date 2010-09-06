using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Eventing.Serializing.Interfaces;
using Eventing.Serializing.Internal;
using ProtoBuf;

namespace Eventing.Serializing
{
    public class EventStoreSerializer
    {
        //CustomBinaryFormatter _ctx = new CustomBinaryFormatter();
        private Dictionary<int, Type> m_ById = new Dictionary<int, Type>();
        private readonly Dictionary<Type, int> m_ByType = new Dictionary<Type, int>();
        private Dictionary<Type, IFormatter> m_Formatters = new Dictionary<Type, IFormatter>();

        public void AutoRegisterSerializationFromAssembly(Assembly ass)
        {
            foreach (Type t in ass.GetTypes().Where(t=>t.GetCustomAttributes(typeof(ProtoBuf.ProtoContractAttribute),false).Count()>0) )
            {
                int hashcode = t.FullName.GetHashCode();
                m_ById.Add(hashcode, t);
                m_ByType.Add(t, hashcode);
                m_Formatters.Add(t,CreateFormatter(t));
            }
        }


        public static IFormatter CreateFormatter(Type type)
        {
            try
            {
                typeof(Serializer)
                        .GetMethod("PrepareSerializer")
                        .MakeGenericMethod(type)
                        .Invoke(null, null);
            }
            catch (TargetInvocationException tie)
            {
                var message = string.Format("Failed to prepare ProtoBuf serializer for '{0}'.", type);
                throw new InvalidOperationException(message, tie.InnerException);
            }

            try
            {
                return (IFormatter)typeof(Serializer)
                        .GetMethod("CreateFormatter")
                        .MakeGenericMethod(type)
                        .Invoke(null, null);
            }
            catch (TargetInvocationException tie)
            {
                var message = string.Format("Failed to create ProtoBuf formatter for '{0}'.", type);
                throw new InvalidOperationException(message, tie.InnerException);
            }
        }


        protected EventStoreSerializer()
        {

        }

        private static EventStoreSerializer m_inst;
        public static EventStoreSerializer Instance
        {
            get
            {
                if (m_inst== null)
                {
                    m_inst = new EventStoreSerializer();
                    //m_inst.AutoRegister();
                }
                return m_inst;
            }
        }

        /*
        Castle.Windsor.IWindsorContainer _container;
        public class Caller
        {
            public string Typename;
            public Type Theclass;
        }

        private List<Caller> _mCallers = new List<Caller>();


                public void RegisterAll(Castle.Windsor.IWindsorContainer container)
                {
                    this._container = container;
                    foreach (var v in AppDomain.CurrentDomain.GetAssemblies())
                        AutoRegister(v);
            
                }
        */
        public object Deserialize(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                //Read type

                IntToBytes itob = new IntToBytes((byte)memoryStream.ReadByte(),
                    (byte)memoryStream.ReadByte(),
                    (byte)memoryStream.ReadByte(),
                    (byte)memoryStream.ReadByte());
                Type t;
                if (!m_ById.TryGetValue(itob.i32, out t))
                    throw new SerializationException(itob.i32 + " has not been registered with the serializer");

                IFormatter form;
                if (!m_Formatters.TryGetValue(t, out form))
                    throw new SerializationException(t.GetType() + " has not been registered with the serializer");

                return form.Deserialize(memoryStream);
                //object obj = FormatterServices.GetUninitializedObject(t);
                //return ProtoBuf.Serializer.Merge(memoryStream, obj);
                //IEvent
                //return _ctx.Deserialize(memoryStream);
            }

        }
        public  byte[] Serialize(object theObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                int key;
                if (!m_ByType.TryGetValue(theObject.GetType(), out key))
                    throw new SerializationException(theObject.GetType() + " has not been registered with the serializer");

                Internal.IntToBytes itob = new IntToBytes(key);
                memoryStream.WriteByte(itob.b0);
                memoryStream.WriteByte(itob.b1);
                memoryStream.WriteByte(itob.b2);
                memoryStream.WriteByte(itob.b3);


                IFormatter form;
                if (!m_Formatters.TryGetValue(theObject.GetType(), out form))
                    throw new SerializationException(theObject.GetType() + " has not been registered with the serializer");
                form.Serialize(memoryStream,theObject);
                //ProtoBuf.Serializer.Serialize(memoryStream, theObject);
                //_ctx.Serialize(memoryStream, theObject);
                return memoryStream.ToArray();
            }
        }

#if(false)
        protected void AutoRegister(Assembly assembly)
        {
            _container.Register(
                 AllTypes
                    .FromAssembly(assembly)
                    .Where(type =>
                        type.BaseType == typeof(Eventing.EventBase)
                    )
                    .Configure(registration =>
                    {
                        registration.LifeStyle.Is(LifestyleType.Transient);
                        registration.Named(registration.Implementation.Name);

                        var c = new Caller();
                        c.Theclass = registration.Implementation;
                        //Get type of first parameter of function Handle
                        c.Typename = registration.Implementation.ToString();
                        _mCallers.Add(c);
                    })
                );
        }
#endif


    }
}
