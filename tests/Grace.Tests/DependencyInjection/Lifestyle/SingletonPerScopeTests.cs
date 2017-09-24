using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    public class SingletonPerScopeTests
    {
        [Fact]
        public void SingletonPerScope_Returns_Same_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerScope();
            });

            IBasicService basicService;

            using (var scope = container.BeginLifetimeScope())
            {
                basicService = scope.Locate<IBasicService>();
                Assert.Same(basicService, scope.Locate<IBasicService>());
            }

            using (var scope = container.BeginLifetimeScope())
            {
                Assert.NotSame(basicService, scope.Locate<IBasicService>());
            }
        }

        [Fact]
        public void SingletonPerScope_Locking_Returns_Same_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerScope(true);
            });

            IBasicService basicService;

            using (var scope = container.BeginLifetimeScope())
            {
                basicService = scope.Locate<IBasicService>();
                Assert.Same(basicService, scope.Locate<IBasicService>());
            }

            using (var scope = container.BeginLifetimeScope())
            {
                Assert.NotSame(basicService, scope.Locate<IBasicService>());
            }
        }

        [Fact]
        public void SingletonPerScope_ShareContext()
        {
            var container = new DependencyInjectionContainer(c => c.SingletonPerScopeShareContext = true);

            container.Configure(c => c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>)).Lifestyle.SingletonPerScope());

            using (var scope = container.BeginLifetimeScope())
            {
                var instance = scope.Locate<IDependentService<int>>(new {intValue = 5});

                Assert.NotNull(instance);
                Assert.Equal(5, instance.Value);
            }
        }
    }
}
