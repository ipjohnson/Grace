using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.MemberInjection
{
    public class MethodInjectionTests
    {
        public class MethodInjectionClass
        {
            public void InjectMethod(IBasicService basicService)
            {
                BasicService = basicService;
            }

            public IBasicService BasicService { get; private set; }
        }

        [Fact]
        public void MethodInjection_OneArg()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MethodInjectionClass>().ImportMethod(m => m.InjectMethod(Arg.Any<IBasicService>()));
            });

            var instance = container.Locate<MethodInjectionClass>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.IsType<BasicService>(instance.BasicService);
        }
    }
}
