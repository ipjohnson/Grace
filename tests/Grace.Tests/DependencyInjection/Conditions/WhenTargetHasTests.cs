using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Conditions
{
    public class WhenTargetHasTests
    {
        public class AttributedTargetClass
        {
            public AttributedTargetClass([SomeTest]IBasicService basicService)
            {
                BasicService = basicService;
            }

            public IBasicService BasicService { get; }
        }


        [Fact]
        public void WhenTargetHas_Parameter_Attributed()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>().When.TargetHas<SomeTestAttribute>();
                c.ExportFactory<IBasicService>(() => new BasicServiceDecorator(new BasicService()));
            });

            var instance = container.Locate<AttributedTargetClass>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.IsType<BasicService>(instance.BasicService);

            var instance2 = container.Locate<DependentService<IBasicService>>();

            Assert.NotNull(instance2);
            Assert.NotNull(instance2.Value);
            Assert.IsType<BasicServiceDecorator>(instance2.Value);
        }
    }
}
