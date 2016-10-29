using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.DependencyInjection.Lifestyle.AncestorClasses;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    public class SingletonPerObjectGraphTests
    {
        [Fact]
        public void SingletonPerObjectGraph_SameInstance_Test()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<LeafClass>().Lifestyle.SingletonPerObjectGraph());

            var root1 = container.Locate<Root>();

            Assert.NotNull(root1);
            Assert.NotNull(root1.Instance1);
            Assert.NotNull(root1.Instance2);
            Assert.NotNull(root1.Instance1.Leaf);
            Assert.NotNull(root1.Instance2.Leaf);

            Assert.NotSame(root1.Instance1, root1.Instance2);
            Assert.Same(root1.Instance1.Leaf, root1.Instance2.Leaf);

            var root2 = container.Locate<Root>();

            Assert.NotNull(root2);
            Assert.NotNull(root2.Instance1);
            Assert.NotNull(root2.Instance2);
            Assert.NotNull(root2.Instance1.Leaf);
            Assert.NotNull(root2.Instance2.Leaf);

            Assert.NotSame(root2.Instance1, root2.Instance2);
            Assert.Same(root2.Instance1.Leaf, root2.Instance2.Leaf);

            Assert.NotSame(root1.Instance1.Leaf, root2.Instance1.Leaf);

        }
    }
}
