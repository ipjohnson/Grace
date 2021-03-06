﻿using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ExportInstance
{
    public class ExportInstanceFuncWithInjectionContextTests
    {
        [Fact]
        public void ExportInstance_With_InjectionContext()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportInstance((scope, staticContext, injectionContext) =>
                {
                    Assert.NotNull(injectionContext);

                    return new BasicService {Count = 5};
                }).As<IBasicService>();
                c.Export<ConstructorImportService>().As<IConstructorImportService>();
            });

            var service = container.Locate<IConstructorImportService>();

            Assert.NotNull(service);
            Assert.Equal(5, service.BasicService.Count);
        }
    }
}
