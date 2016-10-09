using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class DelegateOneArgWrapperTests
    {
        [Fact]
        public void DelegateOneArg_Create_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export(typeof(TwoDependencyService<,>)).As(typeof(ITwoDependencyService<,>));
                c.Export(typeof(DependsOnOneArgDelegate<,>)).As(typeof(IDependsOnOneArgDelegate<,>));
            });

            var instance = container.Locate<IDependsOnOneArgDelegate<IBasicService, int>>();

            var twoService = instance.CreateWithT2(5);

            Assert.NotNull(twoService);
            Assert.NotNull(twoService.Dependency1);
            Assert.IsType<BasicService>(twoService.Dependency1);
            Assert.Equal(5, twoService.Dependency2);
        }
    }
}
