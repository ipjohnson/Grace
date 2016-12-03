using System;
using System.Collections;
using System.Collections.Generic;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection
{
    public class BasicContainerTests
    {
        [Fact]
        public void DependencyInjectionContainer_Create()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            var basicService = container.Locate<IBasicService>();

            Assert.NotNull(basicService);
            Assert.IsType<BasicService>(basicService);
        }

        public class TestModule : IConfigurationModule
        {
            public void Configure(IExportRegistrationBlock registrationBlock)
            {
                registrationBlock.Export<BasicService>().As<IBasicService>();
            }
        }

        [Fact]
        public void DependencyInjectionContainer_Add_Module()
        {
            var container = new DependencyInjectionContainer { new TestModule() };

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
        }
        
        [Fact]
        public void DependencyInjectionContainer_Add_Module_Throws_Exception_For_Null()
        {
            var container = new DependencyInjectionContainer();

            Assert.Throws<ArgumentNullException>(() => container.Add(null));
        }

        [Fact]
        public void DependencyInjectionContainer_Enumerable_Return_Empty()
        {
            var container = new DependencyInjectionContainer { new TestModule() };

            Assert.Empty((IEnumerable<object>)container);
            Assert.Empty((IEnumerable)container);
        }

        public class CustomConfiguration : InjectionScopeConfiguration
        {
            
        }

        [Fact]
        public void DependencyInjectionContainer_Create_Configuration()
        {
            var container = new DependencyInjectionContainer(new CustomConfiguration()) { new TestModule() };

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
        }
    }
}
