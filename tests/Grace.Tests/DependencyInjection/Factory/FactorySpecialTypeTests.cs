using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Factory
{
    public class FactorySpecialTypeTests
    {
        [Fact]
        public void Factory_Depend_On_IExportLocatorScope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportFactory<IExportLocatorScope, IDependentService<IBasicService>>(
                    scope => new DependentService<IBasicService>(scope.Locate<IBasicService>()));
            });

            var service = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(service);
            Assert.NotNull(service.Value);
        }

        [Fact]
        public void Factory_Depend_On_IExportLocatorScope_StaticContext()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFactory<StaticInjectionContext, IBasicService>(context =>
                {
                    Assert.NotNull(context);
                    Assert.Equal(typeof(DependentService<IBasicService>), context.TargetInfo.InjectionType);

                    var basic = new BasicService();

                    return basic;
                });
                c.Export<DependentService<IBasicService>>().As<IDependentService<IBasicService>>();
            });

            var service = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(service);
            Assert.NotNull(service.Value);
        }

        [Fact]
        public void Factory_Depend_On_IExportLocatorScope_IInjectionContext()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFactory<IInjectionContext, IBasicService>(context =>
                {
                    var basic = new BasicService { Count = (int)context.GetExtraData("count") };

                    return basic;
                });
                c.ExportFactory<IExportLocatorScope, IInjectionContext, IDependentService<IBasicService>>(
                    (scope, context) =>
                    {
                        var basicService = scope.Locate<IBasicService>(context);
                        return new DependentService<IBasicService>(basicService);
                    });
            });

            var service = container.Locate<IDependentService<IBasicService>>(new { count = 10 });

            Assert.NotNull(service);
            Assert.NotNull(service.Value);
            Assert.Equal(10, service.Value.Count);
        }
    }
}
