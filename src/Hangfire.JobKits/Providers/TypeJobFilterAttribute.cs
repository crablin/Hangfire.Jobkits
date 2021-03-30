using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Hangfire.JobKits.Providers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class TypeJobFilterAttribute : Attribute
    {
        private ObjectFactory factory;

        public TypeJobFilterAttribute(Type type)
        {
            ImplementationType = type ?? throw new ArgumentNullException(nameof(type));
        }

        public object[] Arguments { get; set; }
        public Type ImplementationType { get; }

        public object CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (factory == null)
            {
                var argumentTypes = Arguments?.Select(a => a.GetType())?.ToArray();
                factory = ActivatorUtilities.CreateFactory(ImplementationType, argumentTypes ?? Type.EmptyTypes);
            }

            return factory(serviceProvider, Arguments);
        }
    }
}
