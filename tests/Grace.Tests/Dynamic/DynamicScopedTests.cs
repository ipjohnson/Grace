using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Dynamic;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.Dynamic
{
    public class DynamicScopedTests
    {
        public class DisposableDependent : IDisposable
        {
            public DisposableDependent(IBasicService basicService)
            {
                BasicService = basicService;
            }

            public IBasicService BasicService { get; }

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
                
            }
        }

        [Fact]
        public void Scoped_DisposalTest()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration());

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerScope());

            var value = container.Locate<DisposableDependent>();

            using (var scope = container.BeginLifetimeScope())
            {
                value = scope.Locate<DisposableDependent>();
            }
        }
    }
}
