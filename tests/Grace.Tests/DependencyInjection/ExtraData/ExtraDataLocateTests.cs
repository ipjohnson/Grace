using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ExtraData
{
    public class ExtraDataLocateTests
    {
        [Fact]
        public void ExtraDataLocate()
        {
            var container = new DependencyInjectionContainer();

            var service = new BasicService();

            var instance = container.Locate<IBasicService>(new { service });

            Assert.Same(service, instance);
        }

        [Fact]
        public void ExtraDataThroughFactory()
        {
            var container = new DependencyInjectionContainer
            {
                c => c.ExportFactory<IExportLocatorScope,IInjectionContext, IDependentService<IBasicService>>(
                    (scope,context) => new DependentService<IBasicService>(scope.Locate<IBasicService>(context)))
            };

            var service = new BasicService();

            var dependentService = container.Locate<IDependentService<IBasicService>>(new { service });

            Assert.NotNull(dependentService);
            Assert.Same(service, dependentService.Value);
        }


        [Fact]
        public void ExtraDataThroughFactoryDependent()
        {
            var container = new DependencyInjectionContainer
            {
                c => c.ExportFactory<IBasicService, IDependentService<IBasicService>>(
                    s => new DependentService<IBasicService>(s))
            };

            var service = new BasicService();

            var dependentService = container.Locate<IDependentService<IBasicService>>(new { service });

            Assert.NotNull(dependentService);
            Assert.Same(service, dependentService.Value);
        }
    }
}
