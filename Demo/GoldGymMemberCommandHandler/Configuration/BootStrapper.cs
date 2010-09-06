using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.ServiceBus.Hosting;

namespace GoldGymMemberCommandHandler.Configuration
{
    class BootStrapper : AbstractBootStrapper
    {
        public BootStrapper()
        {
            log4net.Config.XmlConfigurator.Configure();
            Eventing.Serializing.EventStoreSerializer.Instance.AutoRegisterSerializationFromAssembly(System.Reflection.Assembly.LoadFrom("GymGoldMemberEvents.dll"));
            Eventing.Serializing.EventStoreSerializer.Instance.AutoRegisterSerializationFromAssembly(System.Reflection.Assembly.LoadFrom("GoldGymMemberDomain.dll"));

        }

        public static Castle.Windsor.IWindsorContainer TheContainer;

        protected override void ConfigureContainer()
        {
            TheContainer = container;
            base.ConfigureContainer();
        }


    }
}
