using System;
using Grace.Data;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.LifetimeScope
{
    public class LifetimeScopeTests
    {
        [Fact]
        public void Container_BeingLifetimeScope_SimpleResolve()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
            });

            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var basicService = lifetimeScope.Locate<IBasicService>();

                Assert.NotNull(basicService);
                Assert.IsType<BasicService>(basicService);
            }
        }


        [Fact]
        public void Container_BeingLifetimeScope_LocateOrDefault()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
            });

            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var defaultValue = new BasicService();

                var basicService = lifetimeScope.LocateOrDefault<IBasicService>(defaultValue);

                Assert.NotNull(basicService);
                Assert.IsType<BasicService>(basicService);
                Assert.NotSame(basicService, defaultValue);
            }
        }

        [Fact]
        public void Container_BeingLifetimeScope_LocateOrDefault_ReturnDefault()
        {
            var container = new DependencyInjectionContainer();
            
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var defaultValue = new BasicService();

                var basicService = lifetimeScope.LocateOrDefault<IBasicService>(defaultValue);

                Assert.NotNull(basicService);
                Assert.IsType<BasicService>(basicService);
                Assert.Same(basicService, defaultValue);
            }
        }
        
        [Fact]
        public void Container_BeginLifetimeScope_DisposeCorrectly()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>());

            var disposedCalled = false;

            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var disposed = lifetimeScope.Locate<IDisposableService>();

                Assert.NotNull(disposed);

                disposed.Disposing += (sender, args) => disposedCalled = true;
            }

            Assert.True(disposedCalled);
        }

        [Fact]
        public void Container_BeginLifetimeScope_Locate_ExportLocatorScope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DependentService<IExportLocatorScope>>().Lifestyle.SingletonPerScope());

            using (var scope1 = container.BeginLifetimeScope())
            {
                using (var scope2 = container.BeginLifetimeScope())
                {
                    var instance1 = scope1.Locate<DependentService<IExportLocatorScope>>();
                    var instance2 = scope2.Locate<DependentService<IExportLocatorScope>>();

                    Assert.NotNull(instance1);
                    Assert.NotNull(instance2);
                    Assert.NotSame(instance1, instance2);
                }
            }
        }

        [Fact]
        public void LifetimeScope_Locate()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            using (var scope = container.BeginLifetimeScope())
            {
                var basicService = scope.Locate<IBasicService>();

                Assert.NotNull(basicService);
            }
        }

        [Fact]
        public void LifetimeScope_Locate_Dynamic()
        {
            var container = new DependencyInjectionContainer();

            using (var scope = container.BeginLifetimeScope())
            {
                IBasicService basicService;

                Assert.Throws<LocateException>(() => basicService = scope.Locate<IBasicService>(isDynamic: true));

                container.Configure(c => c.Export<BasicService>().As<IBasicService>());

                basicService = scope.Locate<IBasicService>(isDynamic: true);

                Assert.NotNull(basicService);
            }
        }

        [Fact]
        public void LifetimeScope_TryLocate()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            using (var scope = container.BeginLifetimeScope())
            {
                Assert.True(scope.TryLocate(out IBasicService basicService));

                Assert.False(scope.TryLocate(out IMultipleService multipleService));
            }
        }

        [Fact]
        public void Lifetimescope_CanLocate()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            using (var scope = container.BeginLifetimeScope())
            {
                Assert.True(scope.CanLocate(typeof(IBasicService)));

                Assert.False(scope.CanLocate(typeof(IMultipleService)));
            }
        }

        [Fact]
        public void LifetimeScope_CreateContext()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            using (var scope = container.BeginLifetimeScope())
            {
                var context = scope.CreateContext(new { PropA = 5 });

                Assert.NotNull(context);
                Assert.Equal(5, context.GetExtraDataOrDefaultValue("propa", 0));
            }
        }

        [Fact]
        public void LifetimeScope_LocateAll()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultipleService2>().As<IMultipleService>();
                c.Export<MultipleService3>().As<IMultipleService>();
                c.Export<MultipleService4>().As<IMultipleService>();
                c.Export<MultipleService5>().As<IMultipleService>();
            });


            using (var scope = container.BeginLifetimeScope())
            {
                var list = scope.LocateAll(typeof(IMultipleService));

                Assert.NotNull(list);
                Assert.Equal(5, list.Count);
            }
        }

        [Fact]
        public void LifetimeScope_LocateAll_Generic()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultipleService2>().As<IMultipleService>();
                c.Export<MultipleService3>().As<IMultipleService>();
                c.Export<MultipleService4>().As<IMultipleService>();
                c.Export<MultipleService5>().As<IMultipleService>();
            });

            using (var scope = container.BeginLifetimeScope())
            {
                var list = scope.LocateAll<IMultipleService>();

                Assert.NotNull(list);
                Assert.Equal(5, list.Count);
            }
        }

        [Fact]
        public void LifetimeScope_Locate_Non_Generic()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerScope());

            using (var scope = container.BeginLifetimeScope())
            {
                var instance1 = scope.Locate(typeof(IBasicService), null);
                var instance2 = scope.Locate(typeof(IBasicService), null);

                Assert.Same(instance1, instance2);
            }
        }

        [Fact]
        public void LifetimeScope_Backup_Resolution_Source_With_Name()
        {
            var container = new DependencyInjectionContainer();

            using (var scope = container.BeginLifetimeScope(extraData: new { value = 5 }))
            {
                var instance = scope.Locate<DependentService<int>>();

                Assert.NotNull(instance);
                Assert.Equal(5, instance.Value);
            }
        }


        [Fact]
        public void LifetimeScope_Backup_Resolution_Source_With_Name_Func()
        {
            var container = new DependencyInjectionContainer();

            using (var scope = container.BeginLifetimeScope(extraData: new { value = new Func<int>(() => 5) }))
            {
                var instance = scope.Locate<DependentService<int>>();

                Assert.NotNull(instance);
                Assert.Equal(5, instance.Value);
            }
        }

        [Fact]
        public void LifetimeScope_Backup_Resolution_Source_By_Type()
        {
            var container = new DependencyInjectionContainer();

            using (var scope = container.BeginLifetimeScope(extraData: new { SomeValue = 5 }))
            {
                var instance = scope.Locate<DependentService<int>>();

                Assert.NotNull(instance);
                Assert.Equal(5, instance.Value);
            }
        }

        [Fact]
        public void LifetimeScope_Backup_Resolution_Source_By_Type_Func()
        {
            var container = new DependencyInjectionContainer();

            using (var scope = container.BeginLifetimeScope(extraData: new { SomeValue = new Func<int>(() => 5) }))
            {
                var instance = scope.Locate<DependentService<int>>();

                Assert.NotNull(instance);
                Assert.Equal(5, instance.Value);
            }
        }
    }
}
