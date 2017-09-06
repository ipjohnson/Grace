using Grace.DependencyInjection;
using Grace.Dynamic;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.Dynamic
{
    public class DynamicMemberInitTests
    {
        [Fact]
        public void DynamicMethod_MemberInit_Test()
        {
            var container = new DependencyInjectionContainer(GraceDynamicMethod.Configuration(c =>
            {
                c.Trace = s => Assert.DoesNotContain("falling back", s);
            }));

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<PropertyInjectionService>().As<IPropertyInjectionService>().ImportProperty(i => i.BasicService);
            });

            var instance = container.Locate<IPropertyInjectionService>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
        }
    }
}
