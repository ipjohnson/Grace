using Grace.DependencyInjection;
using Grace.Tests.Classes.Generics;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Generics
{
    public class SimpleGenericTests
    {
        [Fact]
        public void Open_Generic_Locate_Generic_Basic_Service()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(ImportGenericService<>)).As(typeof(IImportGenericService<>));
                c.Export<BasicService>().As<IBasicService>();
            });

            var service = container.Locate<IImportGenericService<IBasicService>>();

            Assert.NotNull(service);
            Assert.NotNull(service.Value);
            Assert.IsType<BasicService>(service.Value);
        }

        [Fact]
        public void Open_Generic_Locate_Generic_Int_Service()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(ImportGenericService<>)).As(typeof(IImportGenericService<>)));

            var service = container.Locate<IImportGenericService<int>>(new { value = 5});

            Assert.NotNull(service);
            Assert.Equal(5, service.Value);
        }
    }
}
