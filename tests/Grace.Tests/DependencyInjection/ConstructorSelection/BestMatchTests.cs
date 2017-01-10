using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ConstructorSelection
{
    public class BestMatchTests
    {
        [Fact]
        public void Container_Match_Best_Constructor_No_Arg()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<MultipleConstructorImport>());

            var service = container.Locate<MultipleConstructorImport>();

            Assert.NotNull(service);
            Assert.Null(service.BasicService);
            Assert.Null(service.ConstructorImportService);
        }

        [Fact]
        public void Container_Match_Best_Constructor_One_Arg()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleConstructorImport>();
                c.Export<BasicService>().As<IBasicService>();
            });

            var service = container.Locate<MultipleConstructorImport>();

            Assert.NotNull(service);
            Assert.NotNull(service.BasicService);
            Assert.Null(service.ConstructorImportService);
        }

        [Fact]
        public void Container_Match_Best_Constructor_Two_Args()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleConstructorImport>();
                c.Export<BasicService>().As<IBasicService>();
                c.Export<ConstructorImportService>().As<IConstructorImportService>();
            });

            var service = container.Locate<MultipleConstructorImport>();

            Assert.NotNull(service);
            Assert.NotNull(service.BasicService);
            Assert.NotNull(service.ConstructorImportService);
        }
    }
}
