using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{

    public class DelegateFactoryTests
    {
        public delegate IBasicService BasicServiceDelegate();

        public delegate ISomePropertyService SomePropertyServiceWithString(string testString);

        public delegate ISomePropertyService SomePropertyServiceWithNone();

        public delegate ISomePropertyService SomePropertyServiceWithBasicService(IBasicService basicService);

        [Fact]
        public void BasicServiceFactoryTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            var bsDelegate = container.Locate<BasicServiceDelegate>();

            Assert.NotNull(bsDelegate);

            var basicService = bsDelegate();

            Assert.NotNull(basicService);
        }

        [Fact]
        public void FactorySomePropertyServiceWithString()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<StringArgSomePropertyService>().As<ISomePropertyService>());

            var factory =
                container.Locate<SomePropertyServiceWithString>();

            Assert.NotNull(factory);

            var service = factory("Hello World");

            Assert.NotNull(service);
            Assert.Equal("Hello World", service.SomeProperty);
        }

        [Fact]
        public void FactorySomePropertyServiceWithNone()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<ReferenceArgSomePropertyService>().As<ISomePropertyService>();
                c.Export<BasicService>().As<IBasicService>();
            });

            var factory =
                container.Locate<SomePropertyServiceWithNone>();

            Assert.NotNull(factory);

            var service = factory();

            Assert.NotNull(service);
            Assert.NotNull(service.SomeProperty);
        }

        [Fact]
        public void FactorySomePropertyServiceWithBasicService()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<ReferenceArgSomePropertyService>().As<ISomePropertyService>();
                c.Export<BasicService>().As<IBasicService>();
            });

            var factory =
                container.Locate<SomePropertyServiceWithBasicService>();

            Assert.NotNull(factory);

            var newBasicService = new BasicService();

            var service = factory(newBasicService);

            Assert.NotNull(service);
            Assert.Same(newBasicService, service.SomeProperty);
        }

        [Fact]
        public void FactoryOneArgWithStringTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<OneArgStringParameterService>().As<IArrayOfObjectsPropertyService>());

            var factory = container.Locate<OneArgStringParameterService.Activate>();

            Assert.NotNull(factory);

            var instance = factory("Blah");

            Assert.NotNull(instance);
            Assert.Single(instance.Parameters);
            Assert.Equal("Blah", instance.Parameters[0]);
        }

        [Fact]
        public void FactoryOneArgWithBasicTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<OneArgRefParameterService>().As<IArrayOfObjectsPropertyService>());

            var factory =
                container.Locate<OneArgRefParameterService.ActivateWithBasicService>();

            Assert.NotNull(factory);

            var basicService = new BasicService();

            var instance = factory(basicService);

            Assert.NotNull(instance);
            Assert.Single(instance.Parameters);
            Assert.Equal(basicService, instance.Parameters[0]);
        }

        [Fact]
        public void FactoryTwoArgWithBasicTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<TwoArgParameterService>().As<IArrayOfObjectsPropertyService>());

            var factory =
                container.Locate<TwoArgParameterService.ActivateWithBasicService>();

            Assert.NotNull(factory);

            var basicService = new BasicService();

            var instance = factory("Blah", basicService);

            Assert.NotNull(instance);

            Assert.Equal(2, instance.Parameters.Length);
            Assert.Equal("Blah", instance.Parameters[0]);
            Assert.Equal(basicService, instance.Parameters[1]);
        }

        [Fact]
        public void FactoryTwoArgWithoutBasicTest()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c =>
            {
                c.Export<TwoArgParameterService>().As<IArrayOfObjectsPropertyService>();
                c.ExportInstance(basicService).As<IBasicService>();
            });

            var factory =
                container.Locate<TwoArgParameterService.ActivateWithOutBasicService>();

            Assert.NotNull(factory);

            var instance = factory("Blah");

            Assert.NotNull(instance);

            Assert.Equal(2, instance.Parameters.Length);
            Assert.Equal("Blah", instance.Parameters[0]);
            Assert.Equal(basicService, instance.Parameters[1]);
        }

        [Fact]
        public void FactoryThreeWithBasicServiceTest()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c => c.Export<ThreeArgParameterService>().As<IArrayOfObjectsPropertyService>());

            var factory =
                container.Locate<ThreeArgParameterService.ActivateWithBasicService>();

            Assert.NotNull(factory);

            var instance = factory("Blah", 5, basicService);

            Assert.NotNull(instance);

            Assert.Equal(3, instance.Parameters.Length);
            Assert.Equal("Blah", instance.Parameters[0]);
            Assert.Equal(5, instance.Parameters[1]);
            Assert.Equal(basicService, instance.Parameters[2]);
        }

        [Fact]
        public void FactoryThreeWithOutBasicServiceTest()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c =>
            {
                c.Export<ThreeArgParameterService>().As<IArrayOfObjectsPropertyService>();
                c.ExportInstance(basicService).As<IBasicService>();
            });

            var factory =
                container.Locate<ThreeArgParameterService.ActivateWithOutBasicService>();

            Assert.NotNull(factory);

            var instance = factory("Blah", 5);

            Assert.NotNull(instance);

            Assert.Equal(3, instance.Parameters.Length);
            Assert.Equal("Blah", instance.Parameters[0]);
            Assert.Equal(5, instance.Parameters[1]);
            Assert.Equal(basicService, instance.Parameters[2]);
        }

        [Fact]
        public void FactoryThreeWithOutBasicServiceOutOfOrderTest()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c =>
            {
                c.Export<ThreeArgParameterService>().As<IArrayOfObjectsPropertyService>();
                c.ExportInstance(basicService).As<IBasicService>();
            });

            var factory =
                container.Locate<ThreeArgParameterService.ActivateWithOutBasicServiceAndOutOfOrder>();

            Assert.NotNull(factory);

            var instance = factory(5, "Blah");

            Assert.NotNull(instance);

            Assert.Equal(3, instance.Parameters.Length);
            Assert.Equal("Blah", instance.Parameters[0]);
            Assert.Equal(5, instance.Parameters[1]);
            Assert.Equal(basicService, instance.Parameters[2]);
        }

        [Fact]
        public void FactoryFourArgWithBasicTest()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c => c.Export<FourArgParameterService>().As<IArrayOfObjectsPropertyService>());

            var factory =
                container.Locate<FourArgParameterService.ActivateWithBasicService>();

            Assert.NotNull(factory);

            var instance = factory("Blah", 5, 9.0, basicService);

            Assert.NotNull(instance);
            Assert.Equal(4, instance.Parameters.Length);
            Assert.Equal("Blah", instance.Parameters[0]);
            Assert.Equal(5, instance.Parameters[1]);
            Assert.Equal(9.0, instance.Parameters[2]);
            Assert.Equal(basicService, instance.Parameters[3]);
        }

        [Fact]
        public void FactoryFourArgWithoutBasicTest()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c =>
            {
                c.Export<FourArgParameterService>().As<IArrayOfObjectsPropertyService>();
                c.ExportInstance(basicService).As<IBasicService>();
            });

            var factory =
                container.Locate<FourArgParameterService.ActivateWithOutBasicService>();

            Assert.NotNull(factory);

            var instance = factory("Blah", 5, 9.0);

            Assert.NotNull(instance);
            Assert.Equal(4, instance.Parameters.Length);
            Assert.Equal("Blah", instance.Parameters[0]);
            Assert.Equal(5, instance.Parameters[1]);
            Assert.Equal(9.0, instance.Parameters[2]);
            Assert.Equal(basicService, instance.Parameters[3]);
        }

        [Fact]
        public void FactoryFourArgWithoutBasicAndOutOfOrderTest()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c =>
            {
                c.Export<FourArgParameterService>().As<IArrayOfObjectsPropertyService>();
                c.ExportInstance(basicService).As<IBasicService>();
            });

            var factory =
                container.Locate<FourArgParameterService.ActivateWithOutBasicServiceAndOutOfOrder>();

            Assert.NotNull(factory);

            var instance = factory(9.0, 5, "Blah");

            Assert.NotNull(instance);
            Assert.Equal(4, instance.Parameters.Length);
            Assert.Equal("Blah", instance.Parameters[0]);
            Assert.Equal(5, instance.Parameters[1]);
            Assert.Equal(9.0, instance.Parameters[2]);
            Assert.Equal(basicService, instance.Parameters[3]);
        }

        [Fact]
        public void FactoryFiveArgWithBasicTest()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c => c.Export<FiveArgParameterService>().As<IArrayOfObjectsPropertyService>());

            var factory =
                container.Locate<FiveArgParameterService.ActivateWithBasicService>();

            Assert.NotNull(factory);

            var instance = factory("Blah", 5, 9.0, 14.0m, basicService);

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Parameters.Length);
            Assert.Equal("Blah", instance.Parameters[0]);
            Assert.Equal(5, instance.Parameters[1]);
            Assert.Equal(9.0, instance.Parameters[2]);
            Assert.Equal(14.0m, instance.Parameters[3]);
            Assert.Equal(basicService, instance.Parameters[4]);
        }

        [Fact]
        public void FactoryFiveArgWithOutBasicTest()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c =>
            {
                c.Export<FiveArgParameterService>().As<IArrayOfObjectsPropertyService>();
                c.ExportInstance(basicService).As<IBasicService>();
            });

            var factory =
                container.Locate<FiveArgParameterService.ActivateWithOutBasicService>();

            Assert.NotNull(factory);

            var instance = factory("Blah", 5, 9.0, 14.0m);

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Parameters.Length);
            Assert.Equal("Blah", instance.Parameters[0]);
            Assert.Equal(5, instance.Parameters[1]);
            Assert.Equal(9.0, instance.Parameters[2]);
            Assert.Equal(14.0m, instance.Parameters[3]);
            Assert.Equal(basicService, instance.Parameters[4]);
        }

        [Fact]
        public void FactoryFiveArgWithOutBasicAndOutOfOrderTest()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c =>
            {
                c.Export<FiveArgParameterService>().As<IArrayOfObjectsPropertyService>();
                c.ExportInstance(basicService).As<IBasicService>();
            });

            var factory =
                container.Locate<FiveArgParameterService.ActivateWithOutBasicServiceAndOutOfOrder>();

            Assert.NotNull(factory);

            var instance = factory(14.0m, "Blah", 9.0, 5);

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Parameters.Length);
            Assert.Equal("Blah", instance.Parameters[0]);
            Assert.Equal(5, instance.Parameters[1]);
            Assert.Equal(9.0, instance.Parameters[2]);
            Assert.Equal(14.0m, instance.Parameters[3]);
            Assert.Equal(basicService, instance.Parameters[4]);
        }

        public class FuncFactoryClass
        {
            private Func<DisposableService> _func;

            public FuncFactoryClass(Func<DisposableService> func)
            {
                _func = func;
            }

            public DisposableService CreateService()
            {
                return _func();
            }
        }

        [Fact]
        public void Keyed_Factory_And_NonKeyed_With_Different_Lifestyle()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<DisposableService>().Lifestyle.SingletonPerNamedScope("CustomScopeName");
                c.Export<DisposableService>().AsKeyed<DisposableService>("TransientKey").ExternallyOwned();
                c.Export<FuncFactoryClass>().WithCtorParam<Func<DisposableService>>().LocateWithKey("TransientKey");
            });

            bool disposedService = false;
            bool disposedTransient = false;

            using (var scope = container.BeginLifetimeScope("CustomScopeName"))
            {
                var service = scope.Locate<DisposableService>();

                Assert.Same(service, scope.Locate<DisposableService>());

                service.Disposing += (sender, args) => disposedService = true;

                var factory = scope.Locate<FuncFactoryClass>();

                var transientService = factory.CreateService();

                Assert.NotSame(service, transientService);

                transientService.Disposing += (sender, args) => disposedTransient = true;
            }

            Assert.True(disposedService);
            Assert.False(disposedTransient);
        }
    }
}
