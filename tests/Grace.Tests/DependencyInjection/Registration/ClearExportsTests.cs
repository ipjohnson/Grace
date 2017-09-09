using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    public class ClearExportsTests
    {
        [Fact]
        public void ClearExport_All()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();

                c.ClearExports();
            });

            Assert.Throws<LocateException>(() => container.Locate<IMultipleService>());
        }
        
        [Fact]
        public void ClearExport_Filtered()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>().WithMetadata("Key", 1);
                c.Export<MultipleService2>().As<IMultipleService>().WithMetadata("Key", 2);
                c.Export<MultipleService3>().As<IMultipleService>().WithMetadata("Key", 3);
                c.Export<MultipleService4>().As<IMultipleService>().WithMetadata("Key", 4);
                c.Export<MultipleService5>().As<IMultipleService>().WithMetadata("Key", 5);

                c.ClearExports(export => export.Metadata.MetadataMatches("Key", 3));
            });

            var services = container.Locate<IMultipleService[]>();

            Assert.Equal(4, services.Length);

            Assert.IsType<MultipleService1>(services[0]);
            Assert.IsType<MultipleService2>(services[1]);
            Assert.IsType<MultipleService4>(services[2]);
            Assert.IsType<MultipleService5>(services[3]);
        }
    }
}
