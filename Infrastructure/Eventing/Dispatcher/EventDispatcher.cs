using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Enumerable = System.Linq.Enumerable;

namespace Eventing.Dispatcher
{
    public class EventDispatcher
    {
        Castle.Windsor.IWindsorContainer _container;
        public class Caller
        {
            public Type Message;
            public Type Theclass;
        }

        public List<Caller> Callers = new List<Caller>();

        public void AutoRegister(Castle.Windsor.IWindsorContainer container, Assembly assembly)
        {
            this._container = container;
            container.Register(
                 AllTypes
                    .FromAssembly(assembly)
                    .Where(type =>
                        typeof(IHandles).IsAssignableFrom(type)
                    )
                    .Configure(registration =>
                    {
                        registration.LifeStyle.Is(LifestyleType.Transient);
                        registration.Named(registration.Implementation.Name);

                        var c = new Caller();
                        c.Theclass = registration.Implementation;
                        //Get type of first parameter of function Handle
                        var method = Enumerable.First<MethodInfo>(registration.Implementation.GetMethods().Where(r => r.Name == "Handle"));
                        ParameterInfo[] i = method.GetParameters();
                        c.Message = i[0].ParameterType;
                        Callers.Add(c);
                    })
                );
        }


        public void Dispatch(IEvent ev)
        {
            foreach (Caller c in Callers.Where(p => p.Message == ev.GetType()))
            {
                object o = _container.Resolve(c.Theclass);
                //Call method on that
                MethodInfo method = c.Theclass.GetMethods().Where(r => r.Name == "Handle").First();
                ParameterInfo[] i = method.GetParameters();
                method.Invoke(o, new object[] { ev });


            }




        }
    }
}
