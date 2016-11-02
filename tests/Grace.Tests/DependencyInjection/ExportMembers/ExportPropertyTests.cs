using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ExportMembers
{
    public class ExportPropertyTests
    {
        public class ExportPropertyClass
        {
            public IBasicService BasicService { get; } = new BasicService();
        }

        [Fact]
        public void ExportProperty_Locate_Property()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<ExportPropertyClass>().ExportMember(e => e.BasicService));

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void ExportProperty_Used_As_Dependency()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<ExportPropertyClass>().ExportMember(e => e.BasicService));

            var instance = container.Locate<DependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
        }
    }
}
