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

            instance = container.Locate<MultipleConstructorImport>(new { basicService = new BasicService()});

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

            instance = container.Locate<DynamicPropertyClassTest>(new { basicService = new BasicService()});

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.Equal(5, instance.IntValue);

            instance = container.Locate<DynamicPropertyClassTest>(new { intValue = 10});

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
    }
}
