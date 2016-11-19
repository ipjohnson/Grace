using System;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Grace.Tests.Classes.Attributes;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection
{
    public class BasicContainerTests
    {
        [Fact]
        public void DependencyInjectionContainer_Create()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());
            
            var basicService = container.Locate<IBasicService>();

            Assert.NotNull(basicService);
            Assert.IsType<BasicService>(basicService);
        }
    }
}
