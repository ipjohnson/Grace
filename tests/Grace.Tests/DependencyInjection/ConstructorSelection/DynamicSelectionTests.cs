using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
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
    }
}
