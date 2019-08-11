using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    public class SingletonPerKeyTest
    {
        [Fact]
        public void SingletonPerKey()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().Lifestyle.SingletonPerKey((scope,context) => context.GetExtraData("key") ?? "A"));

            var instanceA = container.Locate<BasicService>();

            instanceA.Count = 1;

            Assert.Same(instanceA, container.Locate<BasicService>());
            Assert.Same(instanceA, container.Locate<BasicService>(new { key = "A"}));


            var instanceB = container.Locate<BasicService>(new {key = "B"});

            Assert.Same(instanceB, container.Locate<BasicService>(new { key = "B" }));

            Assert.NotSame(instanceA, instanceB);
        }
    }
}
