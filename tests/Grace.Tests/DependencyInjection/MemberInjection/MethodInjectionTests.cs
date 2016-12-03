using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.MemberInjection
{
    public class MethodInjectionTests
    {
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

        [Fact]
        public void MethodInjection_Inject_Members_That_Are_Methods()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MethodInjectionClass>().ImportMembers(MembersThat.AreMethod());
            });

            var instance = container.Locate<MethodInjectionClass>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.IsType<BasicService>(instance.BasicService);

            Assert.NotNull(instance.SecondService);
            Assert.IsType<BasicService>(instance.SecondService);
        }

        [Fact]
        public void MethodInjection_Inject_Members_That_Are_Method_With_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MethodInjectionClass>().ImportMembers(MembersThat.AreMethod(m => m.Name.StartsWith("Some")));
            });

            var instance = container.Locate<MethodInjectionClass>();

            Assert.NotNull(instance);
            Assert.Null(instance.BasicService);

            Assert.NotNull(instance.SecondService);
            Assert.IsType<BasicService>(instance.SecondService);
        }

        [Fact]
        public void MethodInjection_Inject_Int_To_Method()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportInstance(5).AsKeyed<int>("value");
                c.Export<ImportIntMethodClass>().ImportMethod(m => m.SetValue(Arg.Any<int>()));
            });

            var instance = container.Locate<ImportIntMethodClass>();

            Assert.NotNull(instance);

            Assert.Equal(5, instance.Value);
        }
    }
}
