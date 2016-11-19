using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection
{
    public class DefaultValueTests
    {
        [Fact]
        public void DefaultValue_From_Constructor_Used()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAs<OptionalIntServiceConstructor, IOptionalIntServiceConstructor>();
                c.ExportAs<SimpleObjectA, ISimpleObject>();
            });

            var instance = container.Locate<IOptionalIntServiceConstructor>();

            Assert.NotNull(instance);
            Assert.Equal(OptionalIntServiceConstructor.DefaultIntValue, instance.Value);
        }

        [Fact]
        public void DefaultValue_Provided_Used_Over_Constructor()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAs<OptionalIntServiceConstructor, IOptionalIntServiceConstructor>().
                  WithCtorParam<int>().DefaultValue(10);
                c.ExportAs<SimpleObjectA, ISimpleObject>();
            });

            var instance = container.Locate<IOptionalIntServiceConstructor>();

            Assert.NotNull(instance);
            Assert.Equal(10, instance.Value);
        }

        //[Fact]
        //public void DefaulValueBulkRegisterConstructorDefaultValue()
        //{
        //    var container = new DependencyInjectionContainer();

        //    container.Configure(c => c.ExportAs<SimpleObjectA, ISimpleObject>());
        //    container.Configure(c => c.Export(Types.FromThisAssembly(TypesThat.StartWith("Optional"))).
        //                               ByInterfaces().
        //                               WithCtorParam<int>().DefaultValue(t => 5));

        //    var instance = container.Locate<IOptionalIntServiceConstructor>();

        //    Assert.NotNull(instance);
        //    Assert.Equal(5, instance.Value);
        //}
    }
}
