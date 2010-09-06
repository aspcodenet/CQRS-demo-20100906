using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Hosting;

namespace GymGUI.Communications
{
    public class Bus
    {
        protected Bus()
        {
            var thehost = new Rhino.ServiceBus.Hosting.DefaultHost();
            thehost.Start<BootStrapper>();


            _bus = thehost.Container.Resolve<IStartableServiceBus>();

        }
        public IServiceBus ServiceBus
        {
            get { return _bus; }


        }

        private static IServiceBus _bus;

        private static Bus _businstance;
        public static Bus Instance()
        {
            if (_businstance == null)
            {
                _businstance = new Bus();
            }
            return _businstance;
        }
    }

    public class BootStrapper : AbstractBootStrapper
    {
        public BootStrapper()
        {

        }
    }
}
