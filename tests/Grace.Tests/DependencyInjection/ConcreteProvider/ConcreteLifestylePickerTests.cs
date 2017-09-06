using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
using Xunit;

namespace Grace.Tests.DependencyInjection.ConcreteProvider
{
    public class ConcreteLifestylePickerTests
    {
        public class SomeServiceSingleton
        {
            
        }

        [Fact]
        public void ConcreteLifestyleProvider_Singleton()
        {
            var container = new DependencyInjectionContainer(c =>
            {
                c.AutoRegistrationLifestylePicker =
                    type => type.Name.EndsWith("Singleton") ? new SingletonLifestyle() : null;
            });

            var instance = container.Locate<SomeServiceSingleton>();

            var secondInstance = container.Locate<SomeServiceSingleton>();

            Assert.Same(instance, secondInstance);
        }
    }
}
