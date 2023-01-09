using Grace.Data;
using Grace.DependencyInjection;
using Xunit;

namespace Grace.Tests.DependencyInjection.ExtraData
{
    // ReSharper disable once InconsistentNaming
    public class IExtraDataContainerExtensionsTests
    {
        [Fact]
        public void IExtraDataContainerExtensions_GetExtraDataOrDefaultValue_Found()
        {
            var container = new DependencyInjectionContainer();

            container.SetExtraData("Hello", "World");

            var value = container.GetExtraDataOrDefaultValue("Hello", "Bye");

            Assert.Equal("World", value);
        }

        [Fact]
        public void IExtraDataContainerExtensions_GetExtraDataOrDefaultValue_Use_Default()
        {
            var container = new DependencyInjectionContainer();

            container.SetExtraData("Hello", "World");

            var value = container.GetExtraDataOrDefaultValue("Good", "Bye");

            Assert.Equal("Bye", value);
        }

        [Fact]
        public void IExtraDataContainerExtensions_GetExtraDataOrDefaultValue_Convert()
        {
            var container = new DependencyInjectionContainer();

            container.SetExtraData("Hello", "10");

            var value = container.GetExtraDataOrDefaultValue("Hello", 1);

            Assert.Equal(10, value);
        }
    }
}
