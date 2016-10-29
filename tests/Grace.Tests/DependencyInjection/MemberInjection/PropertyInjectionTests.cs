using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.MemberInjection
{
    public class PropertyInjectionTests
    {
        [Fact]
        public void PropertyInjection_ImportMembers()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<PropertyInjectionService>().As<IPropertyInjectionService>().ImportMembers();
            });

            var propertyInjectionService = container.Locate<IPropertyInjectionService>();

            Assert.NotNull(propertyInjectionService);
            Assert.NotNull(propertyInjectionService.BasicService);
        }

        [Fact]
        public void PropertyInjection_ImportProperty()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<PropertyInjectionService>().As<IPropertyInjectionService>().ImportProperty(s => s.BasicService);
            });

            var propertyInjectionService = container.Locate<IPropertyInjectionService>();

            Assert.NotNull(propertyInjectionService);
            Assert.NotNull(propertyInjectionService.BasicService);
        }
    }
}
