using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Xunit;
using Grace.Tests.Classes.Simple;

namespace Grace.Tests.DependencyInjection.ConstructorSelection
{
    public class DynamicSelectionTests
    {
        [Fact]
        public void Dynamic_ConstructorSelection()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.Dynamic);

            var instance = container.Locate<MultipleConstructorImport>();

            Assert.NotNull(instance);
            Assert.Null(instance.BasicService);
            Assert.Null(instance.ConstructorImportService);

            instance = container.Locate<MultipleConstructorImport>(new { basicService = new BasicService() });

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.Null(instance.ConstructorImportService);

            instance = container.Locate<MultipleConstructorImport>(new
            {
                basicService = new BasicService(),
                constructorImportService = new ConstructorImportService(new BasicService())
            });

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.NotNull(instance.ConstructorImportService);
        }

        [Fact]
        public void Dynamic_WithCtorValue()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.Dynamic);

            container.Configure(c => c.Export<MultipleConstructorImport>().WithCtorParam(() => new BasicService()));

            var instance = container.Locate<MultipleConstructorImport>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.Null(instance.ConstructorImportService);

            instance = container.Locate<MultipleConstructorImport>(new ConstructorImportService(new BasicService()));

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.NotNull(instance.ConstructorImportService);
        }

        public class DynamicPropertyClassTest
        {
            public DynamicPropertyClassTest()
            {

            }

            public DynamicPropertyClassTest(IBasicService basicService)
            {
                BasicService = basicService;
            }

            public IBasicService BasicService { get; }

            public int IntValue { get; set; }
        }

        [Fact]
        public void Dynamic_PropertyInject()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.Dynamic);

            container.Configure(c => c.Export<DynamicPropertyClassTest>().ImportProperty(i => i.IntValue).DefaultValue(5));

            var instance = container.Locate<DynamicPropertyClassTest>();

            Assert.NotNull(instance);
            Assert.Null(instance.BasicService);
            Assert.Equal(5, instance.IntValue);

            instance = container.Locate<DynamicPropertyClassTest>(new { basicService = new BasicService() });

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.Equal(5, instance.IntValue);

            instance = container.Locate<DynamicPropertyClassTest>(new { intValue = 10 });

            Assert.NotNull(instance);
            Assert.Null(instance.BasicService);
            Assert.Equal(10, instance.IntValue);

            instance = container.Locate<DynamicPropertyClassTest>(new { intValue = 10, basicService = new BasicService() });

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.Equal(10, instance.IntValue);
        }

        public class DynamicMultipleConstructor_NoDefault
        {
            public DynamicMultipleConstructor_NoDefault(IBasicService basicService)
            {
                BasicService = basicService;
            }

            public DynamicMultipleConstructor_NoDefault(IBasicService basicService, double doubleValue)
            {
                BasicService = basicService;
                DoubleValue = doubleValue;
            }

            public IBasicService BasicService { get; }

            public double DoubleValue { get; }
        }

        [Fact]
        public void Dynamic_ThrowsWhenMissingDependency()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.Dynamic);

            Assert.Throws<LocateException>(() => container.Locate<DynamicMultipleConstructor_NoDefault>());

            var instance = container.Locate<DynamicMultipleConstructor_NoDefault>(new BasicService());

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);

            instance =
                container.Locate<DynamicMultipleConstructor_NoDefault>(new
                {
                    basicService = new BasicService(),
                    doubleValue = 5.0
                });

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.Equal(5.0, instance.DoubleValue);
        }

        public class DynamicMultipleIntParameters
        {
            public DynamicMultipleIntParameters(int firstValue, int secondValue)
            {
                FirstValue = firstValue;
                SecondValue = secondValue;
            }

            public int FirstValue { get; }

            public int SecondValue { get; }
        }

        [Fact]
        public void Dynamic_MultipleIntParameters_Issue78()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.Dynamic);

            var instance = container.Locate<DynamicMultipleIntParameters>(new { secondValue = 10, firstValue = 5 });

            Assert.NotNull(instance);
            Assert.Equal(instance.FirstValue, 5);
            Assert.Equal(10, instance.SecondValue);
        }

        public class MultipleConstructorDisposable : IDisposable
        {
            public MultipleConstructorDisposable(IBasicService basicService)
            {
                BasicService = basicService;
            }

            public MultipleConstructorDisposable(IBasicService basicService, IConstructorImportService constructorImportService)
            {
                BasicService = basicService;
                ConstructorImportService = constructorImportService;
            }

            public IBasicService BasicService { get; }

            public IConstructorImportService ConstructorImportService { get; }

            public event EventHandler Disposed;

            public void Dispose()
            {
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        [Fact]
        public void Dynamic_Disposable()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.Dynamic);

            bool disposed = false;

            using (var owned = container.Locate<Owned<MultipleConstructorDisposable>>(new BasicService()))
            {
                Assert.NotNull(owned);
                Assert.NotNull(owned.Value);
                Assert.NotNull(owned.Value.BasicService);
                Assert.Null(owned.Value.ConstructorImportService);

                owned.Value.Disposed += (sender, args) => disposed = true;
            }

            Assert.True(disposed);

            disposed = false;

            using (var owned = container.Locate<Owned<MultipleConstructorDisposable>>(new { basicService = new BasicService(), constructorImportService = new ConstructorImportService(new BasicService()) }))
            {
                Assert.NotNull(owned);
                Assert.NotNull(owned.Value);
                Assert.NotNull(owned.Value.BasicService);
                Assert.NotNull(owned.Value.ConstructorImportService);

                owned.Value.Disposed += (sender, args) => disposed = true;
            }

            Assert.True(disposed);
        }

        public class ImportTwoMultipleConstructorImport
        {
            public ImportTwoMultipleConstructorImport(MultipleConstructorImport import1, MultipleConstructorImport import2)
            {
                Import1 = import1;
                Import2 = import2;
            }

            public MultipleConstructorImport Import1 { get; }

            public MultipleConstructorImport Import2 { get; }
        }

        [Fact]
        public void Dynamic_PassThroughForPerObjectGraph()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.Dynamic);

            container.Configure(c => c.Export<MultipleConstructorImport>().Lifestyle.SingletonPerObjectGraph());

            var basicService = new BasicService();

            var instance = container.Locate<ImportTwoMultipleConstructorImport>(basicService);

            Assert.NotNull(instance);
            Assert.NotNull(instance.Import1);
            Assert.Same(instance.Import1, instance.Import2);
            Assert.Same(instance.Import1.BasicService, basicService);

            var instance2 = container.Locate<ImportTwoMultipleConstructorImport>(new { basicService, constructorImportService = new ConstructorImportService(basicService) });

            Assert.NotNull(instance2);
            Assert.NotNull(instance2.Import1);
            Assert.Same(instance2.Import1, instance2.Import2);
            Assert.Same(instance2.Import1.BasicService, basicService);
            Assert.NotSame(instance.Import1, instance2.Import1);
        }

        public interface IHaveMultipleConstructors
        {
            int NumProp { get; }

            string StringProp { get; }
        }

        public class HaveMultipleConstructors : IHaveMultipleConstructors
        { 
            public HaveMultipleConstructors()
            {

            }

            public HaveMultipleConstructors(string stringValue, int intValue)
            {

                NumProp = intValue;

                StringProp = stringValue;
            }

            public int NumProp { get; private set; }

            public string StringProp { get; private set; }
        }

        [Fact]
        public void DynamicConstructorSelection_WithFunc()
        {
            DependencyInjectionContainer di = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.Dynamic); // ß not sure about the Dynamic strategy

            di.Configure(config =>
            {
                config.Export<HaveMultipleConstructors>().As<IHaveMultipleConstructors>();
            });

            var myFunc = di.Locate<Func<string, int, IHaveMultipleConstructors>>();

            var functioned = myFunc("funcString", 667);
        }

        public class BasicDependency 
        {
            public BasicDependency(IBasicService basicService, string testString)
            {
                BasicService = basicService;
                TestString = testString;
            }

            public IBasicService BasicService { get; }

            public string TestString { get; }
        }

        [Fact]
        public void DynamicConstructorSelection_WithCtorParam_Func()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.Dynamic); // ß not sure about the Dynamic strategy

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<DependentService<BasicDependency>>()
                        .WithCtorParam((IBasicService service) => new BasicDependency(service, "SomeValue"));
            });

            var instance = container.Locate<DependentService<BasicDependency>>();

            Assert.NotNull(instance);
        }


        [Fact]
        public void DynamicConstructorSelection_InjectionScope_Dependency()
        {
            var container =
                new DependencyInjectionContainer(c =>
                {
                    c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.Dynamic;
                    c.Behaviors.AllowInjectionScopeLocation = true;
                }); // ß not sure about the Dynamic strategy

            container.Configure(c =>
                c.Export<DependentService<IInjectionScope>>().As<IDependentService<IInjectionScope>>());

            var instance = container.Locate<IDependentService<IInjectionScope>>();
        }

        [Fact]
        public void DynamicConstructorSelection_ChildScope()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.Dynamic);

            container.Configure(c => c.Export<DependentService<IBasicService>>().As<IDependentService<IBasicService>>());

            using (var childContainer = container.CreateChildScope(c => c.Export<BasicService>().As<IBasicService>()))
            {
                var instance = childContainer.Locate<IDependentService<IBasicService>>();
            }
        }

        [Fact]
        public void DynamicConstructorSelection_Disposable_Singleton()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.Dynamic);
                
            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>().Lifestyle.Singleton());

            var instance = container.Locate<IDisposableService>();
        }
    }
}
