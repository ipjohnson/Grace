using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Factory
{
    public class FiveArgFactoryTests
    {
        [Fact]
        public void FactoryFiveArg_Create_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(block =>
            {
                block.ExportFactory<SimpleObjectA, SimpleObjectB, SimpleObjectC, SimpleObjectD, SimpleObjectE, IEnumerable<ISimpleObject>>(
                    (a, b, c, d, e) => new List<ISimpleObject> { a, b, c, d, e });
            });

            var list = container.Locate<IEnumerable<ISimpleObject>>();

            Assert.NotNull(list);

            var array = list.ToArray();

            Assert.Equal(5, array.Length);
            Assert.IsType<SimpleObjectA>(array[0]);
            Assert.IsType<SimpleObjectB>(array[1]);
            Assert.IsType<SimpleObjectC>(array[2]);
            Assert.IsType<SimpleObjectD>(array[3]);
            Assert.IsType<SimpleObjectE>(array[4]);

        }
    }
}
