using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
using NSubstitute;
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

        [Fact]
        public void Disposable_DeferredSingleton()
        {
            // Test case for #316

            var service = Substitute.For<IDisposable>();

            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFactory(() => service)
                    .Lifestyle.Custom(new DeferredSingletonLifestyle());
            });

            using (var scope = container.CreateChildScope())
            {
                var located = scope.Locate<IDisposable>();
                Assert.Equal(service, located);
            }

            service.DidNotReceive().Dispose();

            container.Dispose();

            service.Received(1).Dispose();
        }

        internal class Service
        {
            public Service(IDependency dependency) { }
        }

        internal interface IDependency { }

        internal class DependencyImpl : IDependency { }
    }
}
