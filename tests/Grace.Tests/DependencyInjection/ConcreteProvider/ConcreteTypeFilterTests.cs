using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Xunit;

namespace Grace.Tests.DependencyInjection.ConcreteProvider
{
    public class ConcreteTypeFilterTests
    {
        public class SomeClass
        {
            
        }

        [Fact]
        public void ConcreteBaseLine()
        {
            var container = new DependencyInjectionContainer();

            var instance = container.Locate<SomeClass>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void ConcreteTypeFilterByType()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExcludeTypeFromAutoRegistration(typeof(SomeClass)));

            Assert.Throws<LocateException>(() => container.Locate<SomeClass>());
        }


        [Fact]
        public void ConcreteTypeFilterByName()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExcludeTypeFromAutoRegistration(typeof(SomeClass).FullName));

            Assert.Throws<LocateException>(() => container.Locate<SomeClass>());
        }


        [Fact]
        public void ConcreteTypeFilterByFilterNameBeginning()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExcludeTypeFromAutoRegistration("*" + nameof(SomeClass)));

            Assert.Throws<LocateException>(() => container.Locate<SomeClass>());
        }

        [Fact]
        public void ConcreteTypeFilterByFilterNameEnd()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExcludeTypeFromAutoRegistration("Grace.Tests.DependencyInjection.ConcreteProvider.*"));

            Assert.Throws<LocateException>(() => container.Locate<SomeClass>());
        }


        [Fact]
        public void ConcreteTypeFilterByFilterNameContains()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExcludeTypeFromAutoRegistration("*Some*"));

            Assert.Throws<LocateException>(() => container.Locate<SomeClass>());
        }
    }
}
