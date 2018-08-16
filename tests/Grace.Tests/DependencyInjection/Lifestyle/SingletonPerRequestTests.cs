using Grace.DependencyInjection;
using Grace.Dynamic;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    public class SingletonPerRequestTests
    {
        [Fact]
        public void SingletonPerRequest_Without_Provider_Use_PerScope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerRequest());

            IBasicService basicService;

            using (var scope = container.BeginLifetimeScope())
            {
                basicService = scope.Locate<IBasicService>();
                Assert.NotNull(basicService);

                Assert.Same(basicService, scope.Locate<IBasicService>());
            }


            using (var scope = container.BeginLifetimeScope())
            {
                var basicService2 = scope.Locate<IBasicService>();
                Assert.NotNull(basicService2);

                Assert.Same(basicService2, scope.Locate<IBasicService>());
                Assert.NotSame(basicService, basicService2);
            }
        }

        #region test classes

        public interface INameService
        {
            string Name { get; }
        }

        public class AlphaNameService : INameService
        {
            public string Name => "Alpha";
        }

        public class BethaNameService : INameService
        {
            public string Name => "Betha";
        }

        public class MultiNameService
        {
            public INameService Alpha { get; set; }

            public INameService Betha { get; set; }

            public INameService Gamma { get; set; }
        }
        #endregion

        [Fact]
        public void SingletonPerScope_Keyed()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration());

            container.Configure(c =>
            {
                c.Export<AlphaNameService>().AsKeyed<INameService>("Alpha").Lifestyle.SingletonPerRequest();
                c.Export<BethaNameService>().AsKeyed<INameService>("Betha");

                c.Export<MultiNameService>()
                    .ImportProperty(s => s.Alpha).LocateWithKey("Alpha").IsRequired(false)
                    .ImportProperty(s => s.Betha).LocateWithKey("Betha").IsRequired(false)
                    .ImportProperty(s => s.Gamma).LocateWithKey("Gamma").IsRequired(false);
            });


            var multiNameService = container.Locate<MultiNameService>();
            Assert.NotNull(multiNameService);

            Assert.NotNull(multiNameService.Alpha);
            Assert.Equal("Alpha", multiNameService.Alpha.Name);

            Assert.NotNull(multiNameService.Betha);
            Assert.Equal("Betha", multiNameService.Betha.Name);

            Assert.Null(multiNameService.Gamma);
        }

    }
}
