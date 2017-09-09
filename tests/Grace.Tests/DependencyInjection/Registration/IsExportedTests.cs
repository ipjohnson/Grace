using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    public class IsExportedTests
    {
        [Fact]
        public void IsExported()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();

                Assert.False(c.IsExported(typeof(IBasicService)));
                Assert.True(c.IsExported(typeof(IMultipleService)));
            });
        }
        
        [Fact]
        public void IsExported_Keyed()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().AsKeyed<IMultipleService>(5);

                Assert.False(c.IsExported(typeof(IBasicService)));
                Assert.False(c.IsExported(typeof(IMultipleService)));
                Assert.True(c.IsExported(typeof(IMultipleService), 5));
            });
        }

        [Fact]
        public void IsExported_In_Current_Scope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
            });

            container.Configure(c => Assert.True(c.IsExported(typeof(IMultipleService))));
        }

        [Fact]
        public void IsExported_In_Parent()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
            });

            using (var childContainer = container.CreateChildScope())
            {
                childContainer.Configure(c =>
                {
                    Assert.True(c.IsExported(typeof(IMultipleService)));
                });
            }
        }
    }
}
