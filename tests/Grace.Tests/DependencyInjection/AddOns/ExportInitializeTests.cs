using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.AddOns
{
    public class ExportInitializeTests
    {
        [Fact]
        public void ExportInitialize_Type()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportInitialize<BasicService>(service => service.Count = 10);
                c.Export<BasicService>().As<IBasicService>();
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.Equal(10, instance.Count);
        }
        
        [Fact]
        public void ExportInitialize_Interface()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportInitialize<IBasicService>(service => service.Count = 10);
                c.Export<BasicService>();
            });

            var instance = container.Locate<BasicService>();

            Assert.NotNull(instance);
            Assert.Equal(10, instance.Count);
        }

        public interface IInjectable
        {
            void Injected();
        }

        public class ImplementationClass : IInjectable
        {
            public void Injected()
            {
                IntValue = 10;
            }

            public int IntValue { get; set; }
        }

        [Fact]
        public void ExportInitialize_Dependent()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportInitialize<IInjectable>(service => service.Injected());
            });

            var instance = container.Locate<DependentService<ImplementationClass>>();

            Assert.NotNull(instance);
            Assert.Equal(10, instance.Value.IntValue);
        }


        public class InheritBasicService : BasicService
        {
            
        }

        [Fact]
        public void ExportInitialize_Base()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportInitialize<BasicService>(service => service.Count = 10);
                c.Export<InheritBasicService>().As<IBasicService>();
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.Equal(10, instance.Count);
            Assert.IsType<InheritBasicService>(instance);
        }
    }
}
