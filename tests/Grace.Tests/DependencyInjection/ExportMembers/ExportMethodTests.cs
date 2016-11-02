using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ExportMembers
{
    public class ExportMethodTests
    {
        public class ExportMethodClass
        {
            public IDependentService<IBasicService> CreateService(IBasicService basicService)
            {
                return new DependentService<IBasicService>(basicService);
            }
        }

        public class DependentOnServiceClass
        {
            public DependentOnServiceClass(IDependentService<IBasicService> dependentService)
            {
                DependentService = dependentService;
            }

            public IDependentService<IBasicService> DependentService { get; }
        }

        [Fact]
        public void ExportMethod_Locate_ReturnValue()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<ExportMethodClass>().ExportMember(e => e.CreateService(Arg.Any<IBasicService>()));
            });

            var instance = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
        }

        [Fact]
        public void ExportMethod_Used_As_Dependency()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<ExportMethodClass>().ExportMember(e => e.CreateService(Arg.Any<IBasicService>()));
            });

            var instance = container.Locate<DependentOnServiceClass>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.DependentService);
            Assert.NotNull(instance.DependentService.Value);
        }
    }
}
