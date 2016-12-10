using Grace.DependencyInjection;
using Grace.Tests.DependencyInjection.Lifestyle.AncestorClasses;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
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

        [Fact]
        public void SharedPerAncestor_Creates_One_Guaranteed()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<SharedClass>().Lifestyle.SingletonPerAncestor<SomeClass>(locking: true));

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
