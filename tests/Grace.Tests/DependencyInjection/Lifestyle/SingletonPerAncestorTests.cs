using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Lifestyle;
using Grace.Tests.Classes.Simple;
using Grace.Tests.DependencyInjection.Lifestyle.AncestorClasses;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
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
        public void SharedPerAncestor_Ancestor_Not_Found()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerAncestor<DependentService<IBasicService>>();
            });


            var instance = container.Locate<DependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);

            Assert.Throws<LocateException>(() => container.Locate<IBasicService>());
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

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void SharedPerAncestor_Clone(SingletonPerAncestor lifestyle)
        {
            var clone = lifestyle.Clone();

            Assert.IsType<SingletonPerAncestor>(clone);
        }
    }
}
