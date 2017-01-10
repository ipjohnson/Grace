using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    public class RegistrationPriorityTests
    {
        [Fact]
        public void Priority_Registration()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithPriority(1);
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithPriority(2);
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithPriority(5);
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithPriority(4);
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithPriority(3);
            });

            var array = container.Locate<ISimpleObject[]>();

            Assert.NotNull(array);
            Assert.Equal(5, array.Length);
            Assert.IsType<SimpleObjectC>(array[0]);
            Assert.IsType<SimpleObjectD>(array[1]);
            Assert.IsType<SimpleObjectE>(array[2]);
            Assert.IsType<SimpleObjectB>(array[3]);
            Assert.IsType<SimpleObjectA>(array[4]);
        }
    }
}
