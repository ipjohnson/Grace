using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    public class DeferredSingletonLifestyleTests
    {
        [Fact]
        public void DeferredSingleton_CorrectTargetInfo()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(_ =>
            {
                _.ExportFactory<StaticInjectionContext, DependencyImpl>(ctx =>
                {
                    Assert.Equal(RequestType.ConstructorParameter, ctx.TargetInfo.InjectionDependencyType);

                    return new DependencyImpl();
                }).As<IDependency>().Lifestyle.Custom(new DeferredSingletonLifestyle {RootedRequest = false});
            });


            var service = container.Locate<Service>();

            Assert.NotNull(service);
        }

        internal class Service
        {
            public Service(IDependency dependency) { }
        }

        internal interface IDependency { }

        internal class DependencyImpl : IDependency { }
    }
}
