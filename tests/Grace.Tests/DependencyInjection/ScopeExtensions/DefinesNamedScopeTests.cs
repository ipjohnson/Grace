using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ScopeExtensions
{
    public class DefinesNamedScopeTests
    {
        public class DependentClass : IDisposable
        {
            private readonly IDisposable _disposable;
            private IDisposableService _disposableService;

            public DependentClass(IDisposableService disposableService, IDisposable disposable)
            {
                _disposableService = disposableService;
                _disposable = disposable;
            }

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
                _disposable?.Dispose();
            }
        }

        public class GenericDependentClass<T> : IDisposable
        {
            private readonly IDisposable _disposable;
            private T _disposableService;

            public GenericDependentClass(T disposableService, IDisposable disposable)
            {
                _disposableService = disposableService;
                _disposable = disposable;
            }

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
                _disposable?.Dispose();
            }
        }

        [Fact]
        public void DefinesNamedScope_Export()
        {
            var container = new DependencyInjectionContainer();

            var disposed = false;

            container.Configure(c =>
            {
                c.ExportFactory<IExportLocatorScope, IDisposableService>(scope =>
                {
                    Assert.Equal("CustomScope", scope.ScopeName);

                    var service = new DisposableService();

                    service.Disposing += (o, e) => disposed = true;

                    return service;
                });

                c.Export<DependentClass>().ExternallyOwned().DefinesNamedScope("CustomScope");
            });

            Assert.False(disposed);

            var instance = container.Locate<DependentClass>();

            Assert.NotNull(instance);

            instance.Dispose();

            Assert.True(disposed);
        }

        [Fact]
        public void DefinesNamedScope_ExportGeneric()
        {
            var container = new DependencyInjectionContainer();

            var disposed = false;

            container.Configure(c =>
            {
                c.ExportFactory<IExportLocatorScope, IDisposableService>(scope =>
                {
                    Assert.Equal("CustomScope", scope.ScopeName);

                    var service = new DisposableService();

                    service.Disposing += (o, e) => disposed = true;

                    return service;
                });

                c.Export(typeof(GenericDependentClass<>)).ExternallyOwned().DefinesNamedScope("CustomScope");
            });

            Assert.False(disposed);

            var instance = container.Locate<GenericDependentClass<IDisposableService>>();

            Assert.NotNull(instance);

            instance.Dispose();

            Assert.True(disposed);
        }

        [Fact]
        public void DefinesNamedScope_DynamicConstructor()
        {
            var container = new DependencyInjectionContainer(c =>
                c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.Dynamic);

            container.Configure(c =>
            {
                c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>)).ExternallyOwned()
                    .DefinesNamedScope("SomeScope");
            });

            var instance = container.Locate<IDependentService<IExportLocatorScope>>();

            Assert.NotNull(instance);
            Assert.Equal("SomeScope", instance.Value.ScopeName);
        }

    }
}
