using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class MetaTests
    {
        [Fact]
        public void Meta_Get_Container()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().WithMetadata("Hello", "World"));

            var metaObject = container.Locate<Meta<IBasicService>>();

            Assert.NotNull(metaObject.Value);

            var value = metaObject.Metadata["Hello"];

            Assert.Equal("World", value);
        }

        [Fact]
        public void Meta_Import_To_Constructor()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>().WithMetadata("Hello", "World");
                c.Export<ImportBasicServiceMetadata>().As<IImportBasicServiceMetadata>();
            });

            var service = container.Locate<IImportBasicServiceMetadata>();

            Assert.NotNull(service);
            Assert.NotNull(service.BasicService);
            Assert.Equal("World", service.Metadata["Hello"]);
        }
    }
}
