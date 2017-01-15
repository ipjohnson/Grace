using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Extensions;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Factory
{
    public class ExportFuncTests
    {
        [Fact]
        public void ExportFunc_Dependency()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFunc<IBasicService>(scope => new BasicService());
                c.Export<DependentService<IBasicService>>().As<IDependentService<IBasicService>>();
            });

            var instance = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<BasicService>(instance.Value);
        }
    }
}
