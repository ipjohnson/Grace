using System;
using System.Collections.Concurrent;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Generics
{
    public class GenericImplementationForNonGenericInterfaceTests
    {
        
        [Fact]
        public void GenericImplementationForNonGenericInterface()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(BasicServiceGeneric<>)).Lifestyle.Singleton();
                c.ExportGenericInterface<IBasicService>(typeof(BasicServiceGeneric<>));
            });

            var service1 = container.Locate<DependentBasicService1>();
            var service2 = container.Locate<DependentBasicService2>();

            Assert.NotNull(service1);
            Assert.NotNull(service2);

            Assert.NotSame(service1.BasicService, service2.BasicService);
            
            var service1Instance2 = container.Locate<DependentBasicService1>();
            var service2Instance2 = container.Locate<DependentBasicService2>();

            Assert.NotNull(service1Instance2);
            Assert.NotNull(service2Instance2);

            Assert.NotSame(service1Instance2.BasicService, service2Instance2.BasicService);

            Assert.NotSame(service1, service1Instance2);
            Assert.Same(service1.BasicService, service1Instance2.BasicService);

            Assert.NotSame(service2, service2Instance2);
            Assert.Same(service2.BasicService, service2Instance2.BasicService);
        }

        public class BasicServiceGeneric<T> : IBasicService
        {
            /// <inheritdoc />
            public int Count { get; set; }

            /// <inheritdoc />
            public int TestMethod()
            {
                return Count;
            }
        }

        public class DependentBasicService1
        {
            public DependentBasicService1(IBasicService basicService)
            {
                BasicService = basicService;
            }

            public IBasicService BasicService { get; }
        }

        public class DependentBasicService2
        {
            public DependentBasicService2(IBasicService basicService)
            {
                BasicService = basicService;
            }

            public IBasicService BasicService { get; }
        }

    }

    public static class ExportGenericInterfaceClass
    {
        public static IFluentExportInstanceConfiguration<TService> ExportGenericInterface<TService>(this IExportRegistrationBlock block, Type implementationType)
        {
            var typeLookup = new ConcurrentDictionary<Type,Type>();

            return block.ExportFactory<IExportLocatorScope, StaticInjectionContext, TService>((scope, context) =>
            {
                var injectionType = context.TargetInfo.InjectionType;

                var locateType = typeLookup.GetOrAdd(injectionType,
                    _ => implementationType.MakeGenericType(injectionType));

                return (TService)scope.Locate(locateType);
            });
        }
    }
}
