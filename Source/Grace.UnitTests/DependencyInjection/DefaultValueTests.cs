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
    public class DefaultValueTests
    {
        [Fact]
        public void DefaultValueProvidedDuringExportRegistration()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => 
            {
                c.ExportAs<OptionalIntServiceConstructor, IOptionalIntServiceConstructor>().
                  WithCtorParam<int>().DefaultValue(5);
                c.ExportAs<SimpleObjectA, ISimpleObject>();
            });

            var instance = container.Locate<IOptionalIntServiceConstructor>();

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Value);
        }

        [Fact]
        public void DefaulValueBulkRegisterConstructorDefaultValue()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportAs<SimpleObjectA, ISimpleObject>());
            container.Configure(c => c.Export(Types.FromThisAssembly(TypesThat.StartWith("Optional"))).
                                       ByInterfaces().
                                       WithCtorParam<int>().DefaultValue(t => 5));

            var instance = container.Locate<IOptionalIntServiceConstructor>();

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Value);
        }
    }
}
