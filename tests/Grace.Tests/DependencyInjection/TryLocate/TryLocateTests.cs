using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.TryLocate
{
    public class TryLocateTests
    {
        [Fact]
        public void TryLocate_Missing_Interface()
        {
            var container = new DependencyInjectionContainer();

            IBasicService service;

            container.TryLocate(out service);

            Assert.Null(service);
        }
    }
}
