using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.DependencyInjection.Lifestyle.AncestorClasses;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    namespace AncestorClasses
    {
        public class Root
        {
            public Root(SomeClass instance1, SomeClass instance2)
            {
                Instance1 = instance1;
                Instance2 = instance2;
            }

            public SomeClass Instance1 { get; }

            public SomeClass Instance2 { get; }
        }

        public class SomeClass
        {
            public SomeClass(LeafClass leaf)
            {
                Leaf = leaf;
            }

            public LeafClass Leaf { get; }
        }

        public class LeafClass
        {
            public LeafClass(SharedClass shared1, SharedClass shared2)
            {
                Shared1 = shared1;
                Shared2 = shared2;
            }

            public SharedClass Shared1 { get; }

            public SharedClass Shared2 { get; }
        }

        public class SharedClass
        {

        }
    }

    public class SingletonPerAncestorTests
    {
        [Fact]
        public void SharedPerAncestor_Creates_One()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<SharedClass>().Lifestyle.SingletonPerAncestor<SomeClass>());

            var instance = container.Locate<Root>();

            Assert.NotNull(instance);

            Assert.NotNull(instance.Instance1);
            Assert.NotNull(instance.Instance2);
            Assert.NotNull(instance.Instance1.Leaf);
            Assert.NotNull(instance.Instance2.Leaf);
            Assert.NotNull(instance.Instance1.Leaf.Shared1);
            Assert.NotNull(instance.Instance1.Leaf.Shared2);
            Assert.NotNull(instance.Instance2.Leaf.Shared1);
            Assert.NotNull(instance.Instance2.Leaf.Shared2);

            Assert.Same(instance.Instance1.Leaf.Shared1, instance.Instance1.Leaf.Shared2);
            Assert.Same(instance.Instance2.Leaf.Shared1, instance.Instance2.Leaf.Shared2);

            Assert.NotSame(instance.Instance1.Leaf.Shared1, instance.Instance2.Leaf.Shared1);
        }
    }
}
