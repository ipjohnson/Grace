using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ConstructorSelection
{
    public class LeastParametersTests
    {
        [Fact]
        public void LeastParameter_Uses_Correct_Constructor()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection(ConstructorSelectionMethod.LeastParameters));

            container.Configure(c =>
            {
                c.Export<MultipleConstructorImport>();
                c.Export<BasicService>().As<IBasicService>();
                c.Export<ConstructorImportService>().As<IConstructorImportService>();
            });

            var service = container.Locate<MultipleConstructorImport>();

            Assert.NotNull(service);
            Assert.Null(service.BasicService);
            Assert.Null(service.ConstructorImportService);
        }
    }
}
