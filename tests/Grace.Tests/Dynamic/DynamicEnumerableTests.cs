using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.Dynamic;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.Dynamic
{
    public class DynamicEnumerableTests
    {
        [Fact]
        public void DynamicMethod_Enumerable_Test()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            var instance = container.Locate<DependentService<IEnumerable<IBasicService>>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);

            var array = instance.Value.ToArray();

            Assert.NotNull(array);
            Assert.Single(array);
            Assert.IsType<BasicService>(array[0]);
        }

        [Fact]
        public void DynamicMethod_Array_Test()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            var instance = container.Locate<DependentService<IBasicService[]>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);

            var array = instance.Value;

            Assert.NotNull(array);
            Assert.Single(array);
            Assert.IsType<BasicService>(array[0]);
        }
        
    }
}
