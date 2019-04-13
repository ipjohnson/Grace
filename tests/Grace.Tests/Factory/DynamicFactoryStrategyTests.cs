using Grace.DependencyInjection;
using Grace.Factory;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.Factory
{
    public class DynamicFactoryStrategyTests
    {
        public interface IBasicServiceFactory
        {
            IBasicService CreateBasicService();
        }

        [Fact]
        public void DynamicFactoryStrategy_ExportFactory()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportFactory<IBasicServiceFactory>();
            });

            var factory = container.Locate<IBasicServiceFactory>();

            var instance = factory.CreateBasicService();

            Assert.NotNull(instance);

            var instance2 = factory.CreateBasicService();

            Assert.NotNull(instance2);
            Assert.NotSame(instance, instance2);
        }

        public interface IDependentServiceFactory
        {
            IDependentService<int> CreatedDependentService(int value);
        }

        [Fact]
        public void DynamicFactoryStrategy_ExportFactoryWithArg()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>));
                c.ExportFactory<IDependentServiceFactory>();
            });

            var factory = container.Locate<IDependentServiceFactory>();

            var instance = factory.CreatedDependentService(15);

            Assert.NotNull(instance);
            Assert.Equal(15, instance.Value);
        }

        [Fact]
        public void DynamicFactoryStrategy_ExportInterfaceFactories()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>));
                c.ExportInterfaceFactories();
            });

            var factory = container.Locate<IDependentServiceFactory>();

            var instance = factory.CreatedDependentService(15);

            Assert.NotNull(instance);
            Assert.Equal(15, instance.Value);
        }

        public interface IIntDependantClass
        {
            int A { get; }
            int B { get; }
        }

        public class IntDependantClass : IIntDependantClass
        {
            public IntDependantClass(int a, int b)
            {
                A = a;
                B = b;
            }

            public int A { get; }

            public int B { get; }
        }

        public interface IIntDependantClassProvider
        {
            IIntDependantClass Create(int a, int b);
        }


        [Fact]
        public void DynamicFactoryStrategy_ExportFactoryTwoInt()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(IntDependantClass)).As(typeof(IIntDependantClass));
                c.ExportFactory<IIntDependantClassProvider>();
            });

            var factory = container.Locate<IIntDependantClassProvider>();

            var instance = factory.Create(15, 20);

            Assert.NotNull(instance);
            Assert.Equal(15, instance.A);
            Assert.Equal(20, instance.B);
        }


        public interface IIntDependantClassOutOfOrderProvider
        {
            IIntDependantClass Create(int b, int a);
        }


        [Fact]
        public void DynamicFactoryStrategy_ExportFactoryTwoInt_OutOfOrder()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(IntDependantClass)).As(typeof(IIntDependantClass));
                c.ExportFactory<IIntDependantClassOutOfOrderProvider>();
            });

            var factory = container.Locate<IIntDependantClassOutOfOrderProvider>();

            var instance = factory.Create(15, 20);

            Assert.NotNull(instance);
            Assert.Equal(15, instance.B);
            Assert.Equal(20, instance.A);
        }

        public interface IIntDependantClassUnnamedOrderedProvider
        {
            IIntDependantClass Create(int arg1, int arg2);
        }

        [Fact]
        public void DynamicFactoryStrategy_ExportFactoryTwoInt_UnnamedOrdered()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(IntDependantClass)).As(typeof(IIntDependantClass));
                c.ExportFactory<IIntDependantClassUnnamedOrderedProvider>();
            });

            var factory = container.Locate<IIntDependantClassUnnamedOrderedProvider>();

            var instance = factory.Create(15, 20);

            Assert.NotNull(instance);
            Assert.Equal(15, instance.A);
            Assert.Equal(20, instance.B);
        }

        public interface IKeyedBasicServiceProvider
        {
            IBasicService CreateBasicService();

            IBasicService GetTestKey();
        }

        [Fact]
        public void DynamicFactoryStrategy_KeyedProvider()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<CustomBasicService>().AsKeyed<IBasicService>("TestKey");
                c.ExportFactory<IKeyedBasicServiceProvider>();
            });

            var provider = container.Locate<IKeyedBasicServiceProvider>();

            var basicService = provider.CreateBasicService();

            Assert.NotNull(basicService);
            Assert.IsType<BasicService>(basicService);

            var customService = provider.GetTestKey();

            Assert.NotNull(customService);
            Assert.IsType<CustomBasicService>(customService);
        }

        public interface ISomeTestServiceFactory
        {
            ISomeTestService CreateTest();
        }

        public interface ISomeTestService
        {
            ISomeTestService CreateTest();
        }

        public class SomeTestService : ISomeTestService
        {
            private ISomeTestServiceFactory _factory;

            public SomeTestService(ISomeTestServiceFactory factory)
            {
                _factory = factory;
            }

            public ISomeTestService CreateTest()
            {
                return _factory.CreateTest();
            }
        }

        [Fact]
        public void ExportFactory_Recursive()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SomeTestService>().As<ISomeTestService>();
                c.ExportFactory<ISomeTestServiceFactory>();
            });

            var factory = container.Locate<ISomeTestServiceFactory>();

            Assert.NotNull(factory);

            var instance = factory.CreateTest();

            Assert.NotNull(instance);

            var secondInstance = instance.CreateTest();

            Assert.NotNull(secondInstance);
            Assert.NotSame(instance, secondInstance);
        }



    }
}
