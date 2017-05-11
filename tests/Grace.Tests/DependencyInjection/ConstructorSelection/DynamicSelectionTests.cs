using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
