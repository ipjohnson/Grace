using Grace.DependencyInjection;
using Xunit;

namespace Grace.Tests.DependencyInjection.LifetimeScope
{
    public class ScopedExtraDataTests
    {
        public class Top
        {
            public Top(Sub subInstance)
            {
                SubInstance = subInstance;
            }

            public Sub SubInstance { get; } 
        }


        public class Sub
        {

        }

        [Fact]
        public void ScopeExtraDataTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(block =>
            {
                block.ExcludeTypeFromAutoRegistration(typeof(Sub));
                block.Export<Top>();
            });

            var subInstance = new Sub();

            using (var scope = container.BeginLifetimeScope(extraData: new {subInstance}))
            {
                var firstTop = scope.Locate<Top>();

                var secondTop = scope.Locate<Top>();

                Assert.NotSame(firstTop, secondTop);

                Assert.Same(firstTop.SubInstance, secondTop.SubInstance);
            }
        }

        public class Accessor<T>
        {
            public T Instance { get; set; }
        }

        /// <summary>
        /// This shows how to use it with an accessor idea vs. using the container
        /// </summary>
        [Fact]
        public void ScopedExtraDataUsingAccessor()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(block =>
            {
                block.Export(typeof(Accessor<>)).Lifestyle.SingletonPerScope();
                block.ExportAsKeyed<Sub, Sub>("Sub");
                block.ExportFactory((Accessor<Sub> accessor, IExportLocatorScope scope) => 
                    accessor.Instance ?? (accessor.Instance =  scope.Locate<Sub>(withKey: "Sub")));
                
            });

            using (var scope = container.BeginLifetimeScope())
            {
                var firstTop = scope.Locate<Top>();

                var secondTop = scope.Locate<Top>();

                Assert.NotSame(firstTop, secondTop);

                Assert.Same(firstTop.SubInstance, secondTop.SubInstance);

            }
        }

    }
}
