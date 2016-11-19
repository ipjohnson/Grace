using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ExportTypeSet
{
    public class ByInterfaceTests
    {
        [Fact]
        public void Export_Type_Set_By_Interface()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAssemblyContaining<ByInterfaceTests>().ByInterface<IMultipleService>();
            });

            var enumerable = container.Locate<IEnumerable<IMultipleService>>();

            Assert.NotNull(enumerable);

            var array = enumerable.ToArray();

            Assert.NotNull(array);
            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService4>(array[3]);
            Assert.IsType<MultipleService5>(array[4]);
        }
    }
}
