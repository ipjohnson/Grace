using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Conditions
{
    public class WhenMemberHasTests
    {
        public class AttributedMemberClass
        {
            [SomeTest]
            public IBasicService BasicService { get; set; }
        }

        [Fact]
        public void WhenMemberHas_Property_Attributed()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>().When.MemberHas<SomeTestAttribute>();
                c.ExportFactory<IBasicService>(() => new BasicServiceDecorator(new BasicService()));
                c.Export<AttributedMemberClass>().ImportProperty(m => m.BasicService);
            });

            var instance = container.Locate<AttributedMemberClass>();
            
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
