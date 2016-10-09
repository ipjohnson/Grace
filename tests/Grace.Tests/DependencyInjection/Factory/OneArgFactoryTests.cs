using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Factory
{
    public class OneArgFactoryTests
    {
        [Fact]
        public void FactoryOneArg_Create_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportFactory<IBasicService, IConstructorImportService>(
                    service => new ConstructorImportService(service));
            });

            var instance = container.Locate<IConstructorImportService>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.IsType<BasicService>(instance.BasicService);
        }
    }
}
