using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ConstructorSelection
{
    public class MostParametersTests
    {
        // re-enable this test when injection context is implemented
        [Fact]
        public void MostParameters_Uses_Correct_Constructor()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.MostParameters);

            container.Configure(c =>
            {
                c.Export<MultipleConstructorImport>();
            });

            Assert.ThrowsAny<Exception>(() => container.Locate<MultipleConstructorImport>());

            Assert.ThrowsAny<Exception>(() => container.Locate<MultipleConstructorImport>(new { Service = new BasicService() }));

            var basicService = new BasicService();

            var constructorImporter = new ConstructorImportService(basicService);

            var service = container.Locate<MultipleConstructorImport>(new { basicService, constructorImporter });

            Assert.NotNull(service);
            Assert.Same(basicService, service.BasicService);
            Assert.Same(constructorImporter, service.ConstructorImportService);
        }
    }
}
