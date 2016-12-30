using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    public class SingletonPerRequestTests
    {
        [Fact]
        public void SingletonPerRequest_Without_Provider_Use_PerScope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerRequest());

            IBasicService basicService;

            using (var scope = container.BeginLifetimeScope())
            {
                basicService = scope.Locate<IBasicService>();
                Assert.NotNull(basicService);

                Assert.Same(basicService, scope.Locate<IBasicService>());
            }


            using (var scope = container.BeginLifetimeScope())
            {
                var basicService2 = scope.Locate<IBasicService>();
                Assert.NotNull(basicService2);

                Assert.Same(basicService2, scope.Locate<IBasicService>());
                Assert.NotSame(basicService, basicService2);
            }
        }


    }
}
