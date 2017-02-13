using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.DisposableTests
{
    public class DisposableTransientTrackingTests
    {
        [Fact]
        public void DisposableTransientTrack_TurnedOn_Export()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>());

            var disposableInstance = container.Locate<IDisposableService>();

            var disposed = false;

            disposableInstance.Disposing += (sender, args) => disposed = true;

            container.Dispose();

            Assert.True(disposed);
        }

        [Fact]
        public void DisposableTransientTrack_TurnedOff_Export()
        {
            var container = new DependencyInjectionContainer(c => c.TrackDisposableTransients = false);

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>());

            var disposableInstance = container.Locate<IDisposableService>();

            var disposed = false;

            disposableInstance.Disposing += (sender, args) => disposed = true;

            container.Dispose();

            Assert.False(disposed);
        }

        [Fact]
        public void DisposableTransientTrack_TurnedOn_ExportFactory()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportFactory(() => new DisposableService()).As<IDisposableService>());

            var disposableInstance = container.Locate<IDisposableService>();

            var disposed = false;

            disposableInstance.Disposing += (sender, args) => disposed = true;

            container.Dispose();

            Assert.True(disposed);
        }

        [Fact]
        public void DisposableTransientTrack_TurnedOff_ExportFactory()
        {
            var container = new DependencyInjectionContainer(c => c.TrackDisposableTransients = false);

            container.Configure(c => c.ExportFactory(() => new DisposableService()).As<IDisposableService>());

            var disposableInstance = container.Locate<IDisposableService>();

            var disposed = false;

            disposableInstance.Disposing += (sender, args) => disposed = true;

            container.Dispose();

            Assert.False(disposed);
        }

        [Fact]
        public void DisposableTransientTrack_TurnedOn_TransientLifestyle()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>));
                c.Export<DisposableService>().As<IDisposableService>().Lifestyle.SingletonPerObjectGraph();
            });

            var instance = container.Locate<IDependentService<IDisposableService>>();

            var disposed = false;

            instance.Value.Disposing += (sender, args) => disposed = true;

            container.Dispose();

            Assert.True(disposed);
        }


        [Fact]
        public void DisposableTransientTrack_TurnedOff_TransientLifestyle()
        {
            var container = new DependencyInjectionContainer(c => c.TrackDisposableTransients = false);

            container.Configure(c =>
            {
                c.Export(typeof(DependentService<>)).As(typeof(IDependentService<>));
                c.Export<DisposableService>().As<IDisposableService>().Lifestyle.SingletonPerObjectGraph();
            });

            var instance = container.Locate<IDependentService<IDisposableService>>();

            var disposed = false;

            instance.Value.Disposing += (sender, args) => disposed = true;

            container.Dispose();

            Assert.False(disposed);
        }
    }
}
