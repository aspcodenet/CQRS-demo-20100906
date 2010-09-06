using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;

namespace ReadModelUpdater
{
    class Program
    {
        public static ILog log = log4net.LogManager.GetLogger("CommandHandler");
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            Castle.Windsor.IWindsorContainer container = new Castle.Windsor.WindsorContainer(new Castle.Windsor.Configuration.Interpreters.XmlInterpreter());
             

            var dispatcher = new Eventing.Dispatcher.EventDispatcher();
            dispatcher.AutoRegister(container, System.Reflection.Assembly.GetExecutingAssembly());

            Eventing.Serializing.EventStoreSerializer.Instance.AutoRegisterSerializationFromAssembly(Assembly.LoadFrom("GymGoldMemberEvents.dll"));


            var logchaser = new LogChaser("gymgold-report", dispatcher, container);
            while (true)
            {
                int cnt = logchaser.Chase();
                if ( cnt == 0)
                    System.Threading.Thread.Sleep(5000);
            }




        }
    }
}
