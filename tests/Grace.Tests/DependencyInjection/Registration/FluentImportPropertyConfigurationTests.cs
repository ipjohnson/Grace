using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    public class FluentImportPropertyConfigurationTests
    {
        public class ImportPropertyClass<T>
        {
            public T ImportProperty { get; set; }
        }

        [Fact]
        public void FluentImportPropertyConfiguration_Consider()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultipleService2>().As<IMultipleService>();
                c.Export<MultipleService3>().As<IMultipleService>();
                c.Export<MultipleService4>().As<IMultipleService>();
                c.Export<MultipleService5>().As<IMultipleService>();
                c.Export<ImportPropertyClass<IMultipleService>>()
                    .ImportProperty(p => p.ImportProperty)
                    .Consider(strategy => strategy.ActivationType.Name.EndsWith("3"));
            });

            var instance = container.Locate<ImportPropertyClass<IMultipleService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.ImportProperty);
            Assert.IsType<MultipleService3>(instance.ImportProperty);
        }


        [Fact]
        public void FluentImportPropertyConfiguration_LocateKey()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().AsKeyed<IMultipleService>(1);
                c.Export<MultipleService2>().AsKeyed<IMultipleService>(2);
                c.Export<MultipleService3>().AsKeyed<IMultipleService>(3);
                c.Export<MultipleService4>().AsKeyed<IMultipleService>(4);
                c.Export<MultipleService5>().AsKeyed<IMultipleService>(5);
                c.Export<ImportPropertyClass<IMultipleService>>()
                    .ImportProperty(p => p.ImportProperty).LocateWithKey(3);
            });

            var instance = container.Locate<ImportPropertyClass<IMultipleService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.ImportProperty);
            Assert.IsType<MultipleService3>(instance.ImportProperty);
        }

        [Fact]
        public void FluentImportPropertyConfiguration_IsRequired()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<ImportPropertyClass<IMultipleService>>()
                    .ImportProperty(p => p.ImportProperty).IsRequired(false);
            });

            var instance = container.Locate<ImportPropertyClass<IMultipleService>>();

            Assert.NotNull(instance);
            Assert.Null(instance.ImportProperty);
        }
    }
}
