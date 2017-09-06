using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    public class OnlyIfExportTests
    {
        [Fact]
        public void ExportFactory_IfNotRegistered_Pass()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFactory<IBasicService>(() => new BasicServiceDecorator(new BasicService())).IfNotRegistered(typeof(IBasicService));
                c.Export<BasicService>().As<IBasicService>();
            });

            var array = container.Locate<IBasicService[]>();

            Assert.Equal(2, array.Length);
            Assert.IsType<BasicServiceDecorator>(array[0]);
            Assert.IsType<BasicService>(array[1]);
        }

        [Fact]
        public void ExportFactory_IfNotRegistered_Fail()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportFactory<IBasicService>(() => new BasicServiceDecorator(new BasicService())).IfNotRegistered(typeof(IBasicService));
            });

            var array = container.Locate<IBasicService[]>();

            Assert.Equal(1, array.Length);
            Assert.IsType<BasicService>(array[0]);
        }

        [Fact]
        public void Export_IfNotRegistered_Pass()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicServiceDecorator>().As<IBasicService>().IfNotRegistered(typeof(IBasicService));
                c.Export<BasicService>().As<IBasicService>();
            });

            var array = container.Locate<IBasicService[]>();

            Assert.Equal(2, array.Length);
            Assert.IsType<BasicServiceDecorator>(array[0]);
            Assert.IsType<BasicService>(array[1]);
        }
        
        [Fact]
        public void Export_IfNotRegistered_Fail()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<BasicServiceDecorator>().As<IBasicService>().IfNotRegistered(typeof(IBasicService));
            });

            var array = container.Locate<IBasicService[]>();

            Assert.Equal(1, array.Length);
            Assert.IsType<BasicService>(array[0]);
        }


        [Fact]
        public void ExportNonGeneric_IfNotRegistered_Pass()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(BasicServiceDecorator)).As(typeof(IBasicService)).IfNotRegistered(typeof(IBasicService));
                c.Export<BasicService>().As<IBasicService>();
            });

            var array = container.Locate<IBasicService[]>();

            Assert.Equal(2, array.Length);
            Assert.IsType<BasicServiceDecorator>(array[0]);
            Assert.IsType<BasicService>(array[1]);
        }

        [Fact]
        public void ExportNonGeneric_IfNotRegistered_Fail()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export(typeof(BasicServiceDecorator)).As(typeof(IBasicService)).IfNotRegistered(typeof(IBasicService));
            });

            var array = container.Locate<IBasicService[]>();

            Assert.Equal(1, array.Length);
            Assert.IsType<BasicService>(array[0]);
        }
    }
}
