using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ExportMembers
{
    public class ExportFieldTests
    {
        public class ExportFieldClass
        {
            public IBasicService BasicService = new BasicService();
        }

        [Fact]
        public void ExportField_Locate_Field()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<ExportFieldClass>().ExportMember(e => e.BasicService));

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void ExportField_Used_As_Dependency()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<ExportFieldClass>().ExportMember(e => e.BasicService));

            var instance = container.Locate<DependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
        }
    }
}
