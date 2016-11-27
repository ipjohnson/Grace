using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    public class SingletonTests
    {
        [Fact]
        public void Singleton_Return_Same_Insance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.Singleton());

            var instance1 = container.Locate<IBasicService>();
            var instance2 = container.Locate<IBasicService>();

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void Singleton_Import_Returns_Same_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>().Lifestyle.Singleton();
                c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>));
            });

            var instance1 = container.Locate<IDependentService<IBasicService>>();
            var instance2 = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(instance1);
            Assert.NotNull(instance2);

            Assert.NotSame(instance1, instance2);
            Assert.Same(instance1.Value, instance2.Value);
        }
    }
}
