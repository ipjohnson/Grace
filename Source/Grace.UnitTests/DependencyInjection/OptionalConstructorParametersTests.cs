using Grace.DependencyInjection;
using Grace.UnitTests.Classes.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
    public class OptionalConstructorParametersTests
    {
        [Fact]
        public void OptionalConstructorISimpleObjectTest()
        {
            var container = new DependencyInjectionContainer();
            container.Configure(c => c.ExportAs<OptionalServiceConstructor, IOptionalServiceConstructor>());

            var instance = container.Locate<IOptionalServiceConstructor>();
            Assert.NotNull(instance);
        }

        [Fact]
        public void OptionalConstructorIntTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => 
            {
                c.ExportAs<OptionalIntServiceConstructor, IOptionalIntServiceConstructor>();
                c.ExportAs<SimpleObjectA, ISimpleObject>();
            });

            var instance = container.Locate<IOptionalIntServiceConstructor>();
            Assert.NotNull(instance);
        }
    }
}
