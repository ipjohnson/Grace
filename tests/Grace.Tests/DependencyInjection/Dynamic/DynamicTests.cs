using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Dynamic
{
    public class DynamicTests
    {
        [Fact]
        public void Dynamic_Constructor_Parameter_Resolve_From_Child_Scope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>)).WithCtorParam<object>().IsDynamic());

            using (var childScope = container.CreateChildScope(c => c.Export<BasicService>().As<IBasicService>()))
            {
                var instance = childScope.Locate<IDependentService<IBasicService>>();

                Assert.NotNull(instance);
                Assert.NotNull(instance.Value);
            }
        }
    }
}
