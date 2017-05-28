using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    public class IsExportedTests
    {
        [Fact]
        public void IsExported_Test()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();

                Assert.False(c.IsExported(typeof(IBasicService)));
                Assert.True(c.IsExported(typeof(IMultipleService)));
            });
        }
        
        [Fact]
        public void IsExported_Keyed_Test()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().AsKeyed<IMultipleService>(5);

                Assert.False(c.IsExported(typeof(IBasicService)));
                Assert.False(c.IsExported(typeof(IMultipleService)));
                Assert.True(c.IsExported(typeof(IMultipleService), 5));
            });
        }
    }
}
