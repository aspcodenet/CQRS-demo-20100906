using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.ServiceBus.Hosting;

namespace GoldGymMemberCommandHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new RemoteAppDomainHost(typeof(Configuration.BootStrapper));

            try
            {
                host.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



            Console.WriteLine("Starting to process messages");
            Console.ReadLine();
        }
    }
}
