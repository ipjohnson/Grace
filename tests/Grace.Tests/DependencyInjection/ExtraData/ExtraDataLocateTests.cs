using System;
using System.Linq.Expressions;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
using Grace.Tests.Classes.Generics;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ExtraData
{
    public class ExtraDataLocateTests
    {
        [Fact]
        public void ExtraDataLocate()
        {
            var container = new DependencyInjectionContainer();

            var service = new BasicService();

            var instance = container.Locate<IBasicService>(new { service });

            Assert.Same(service, instance);
        }

        [Fact]
        public void ExtraDataThroughFactory()
        {
            var container = new DependencyInjectionContainer
            {
                c => c.ExportFactory<IExportLocatorScope,IInjectionContext, IDependentService<IBasicService>>(
                    (scope,context) => new DependentService<IBasicService>(scope.Locate<IBasicService>(context)))
            };

            var service = new BasicService();

            var dependentService = container.Locate<IDependentService<IBasicService>>(new { service });

            Assert.NotNull(dependentService);
            Assert.Same(service, dependentService.Value);
        }


        [Fact]
        public void ExtraDataThroughFactoryDependent()
        {
            var container = new DependencyInjectionContainer
            {
                c => c.ExportFactory<IBasicService, IDependentService<IBasicService>>(
                    s => new DependentService<IBasicService>(s))
            };

            var service = new BasicService();

            var dependentService = container.Locate<IDependentService<IBasicService>>(new { service });

            Assert.NotNull(dependentService);
            Assert.Same(service, dependentService.Value);
        }

        [Fact]
        public void GenericExtraData()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExcludeTypeFromAutoRegistration("Grace.*");
                c.Export(typeof(GenericServiceA<>)).As(typeof(IGenericServiceA<>));
            });

            var instance = container.Locate<IGenericServiceA<int>>(new {serivce= new GenericServiceB<int>()});
        }

        [Fact]
        public void UseCase1()
        {
            var container = new DependencyInjectionContainer();
            container.Configure(x =>
            {
                x.ExcludeTypeFromAutoRegistration(typeof(Filter<>));
                x.Export(typeof(MyClass<>)).Lifestyle.Custom(new DeferredSingletonLifestyle());
                x.Export<SomeService>().Lifestyle.Custom(new DeferredSingletonLifestyle());
            });
            var test = new {blah = 1};

            var filter = new Filter<int>(x => x);
            var myInstance = container.Locate<MyClass<int>>(extraData: filter);
        }

        class MyClass<T>
        {
            public MyClass(SomeService service, Filter<T> filter)
            {
            }
        }

        class SomeService
        {
        }

        public class Filter<T>
        {
            public Expression<Func<T, int>> Expression { get; }

            public Filter(Expression<Func<T, int>> selector)
            {
                Expression = selector;
            }
        }
    }
}
