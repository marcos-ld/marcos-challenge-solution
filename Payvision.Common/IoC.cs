using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Payvision.Common
{
    public static class Ioc
    {
        private static IContainer _applicationContainer;
        public static IContainer ApplicationContainer => _applicationContainer ?? RegisterComponents();

        public static IContainer RegisterComponents()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Service.BitService)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            _applicationContainer = builder.Build();

            return _applicationContainer;
        }
    }
}
