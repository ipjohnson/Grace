using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ExportInstance
{
    public class ExportInstanceFuncWithScopeTests
    {
        [Fact]
        public void ExportInstance_With_Scope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportInstance(scope => new BasicService { Count = 5 }).As<IBasicService>();
                c.Export<ConstructorImportService>().As<IConstructorImportService>();
            });

            var service = container.Locate<IConstructorImportService>();

            Assert.NotNull(service);
            Assert.Equal(5, service.BasicService.Count);
        }

        [Fact]
        public void ExportInstance_Correct_Scope_Returned()
        {
            var container = new DependencyInjectionContainer();

            var currentId = Guid.Empty;

            container.Configure(c =>
            {
                c.ExportInstance(scope =>
                {
                    currentId = scope.ScopeId;

                    return new BasicService();
                }).As<IBasicService>();
                c.Export<ConstructorImportService>().As<IConstructorImportService>();
            });

            using (var childScope = container.BeginLifetimeScope())
            {
                var instance = childScope.Locate<IConstructorImportService>();

                Assert.NotNull(instance);

                Assert.Equal(childScope.ScopeId, currentId);
            }
        }
    }
}
