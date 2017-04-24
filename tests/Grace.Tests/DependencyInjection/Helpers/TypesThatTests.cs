using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Attributes;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Helpers
{
    public class TypesThatTests
    {
        [Fact]
        public void AreInTheSameNamespace()
        {
            Func<Type, bool> sameNamespace = TypesThat.AreInTheSameNamespaceAs(typeof(DependencyInjectionContainer));

            Assert.True(sameNamespace(typeof(TypesThat)));

            Assert.False(sameNamespace(GetType()));
        }

        [Fact]
        public void AreInTheSameNamespaceAndSubnamespace()
        {
            Func<Type, bool> sameNamespace = TypesThat.AreInTheSameNamespaceAs(typeof(DependencyInjectionContainer), true);

            Assert.True(sameNamespace(typeof(TypesThat)));

            Assert.False(sameNamespace(GetType()));
        }

        [Fact]
        public void AreInTheSameNamespaceGeneric()
        {
            Func<Type, bool> sameNamespace = TypesThat.AreInTheSameNamespaceAs<DependencyInjectionContainer>();

            Assert.True(sameNamespace(typeof(TypesThat)));

            Assert.False(sameNamespace(GetType()));
        }

        [Fact]
        public void AreInTheSameNamespaceAndSubnamespaceGeneric()
        {
            Func<Type, bool> sameNamespace = TypesThat.AreInTheSameNamespaceAs<DependencyInjectionContainer>(true);

            Assert.True(sameNamespace(typeof(TypesThat)));

            Assert.False(sameNamespace(GetType()));
        }

        [Fact]
        public void HaveAttributeType()
        {
            Func<Type, bool> haveFilter = TypesThat.HaveAttribute(typeof(SomeTestAttribute));

            Assert.True(haveFilter(typeof(AttributedSimpleObjectA)));

            Assert.False(haveFilter(typeof(AttributedSimpleObjectE)));
        }

        [Fact]
        public void HaveAttributeTypeFiltered()
        {
            Func<Type, bool> haveFilter = TypesThat.HaveAttribute(typeof(SomeTestAttribute),
                x => ((SomeTestAttribute)x).TestValue == 5);

            Assert.True(haveFilter(typeof(AttributedSimpleObjectA)));

            Assert.False(haveFilter(typeof(AttributedSimpleObjectB)));

            Assert.False(haveFilter(typeof(AttributedSimpleObjectE)));
        }

        [Fact]
        public void HaveAttributeGeneric()
        {
            Func<Type, bool> haveFilter = TypesThat.HaveAttribute(typeof(SomeTestAttribute));

            Assert.True(haveFilter(typeof(AttributedSimpleObjectA)));

            Assert.False(haveFilter(typeof(AttributedSimpleObjectE)));
        }

        [Fact]
        public void HaveAttributeGenericFiltered()
        {
            Func<Type, bool> haveFilter = TypesThat.HaveAttribute<SomeTestAttribute>(x => x.TestValue == 5);

            Assert.True(haveFilter(typeof(AttributedSimpleObjectA)));

            Assert.False(haveFilter(typeof(AttributedSimpleObjectB)));

            Assert.False(haveFilter(typeof(AttributedSimpleObjectE)));
        }

        [Fact]
        public void OrFilteredTest()
        {
            Func<Type, bool> haveFilter = TypesThat.EndWith("A").Or.EndWith("B");

            Assert.True(haveFilter(typeof(AttributedSimpleObjectA)));

            Assert.True(haveFilter(typeof(AttributedSimpleObjectB)));

            Assert.False(haveFilter(typeof(AttributedSimpleObjectC)));
        }

        [Fact]
        public void TypesThat_And()
        {
            Func<Type, bool> haveFilter = TypesThat.StartWith("Attributed").And.EndWith("B");

            Assert.False(haveFilter(typeof(AttributedSimpleObjectA)));

            Assert.True(haveFilter(typeof(AttributedSimpleObjectB)));

        }

        [Fact]
        public void TypesThat_StartWith()
        {
            Func<Type, bool> haveFilter = TypesThat.StartWith("A");

            Assert.True(haveFilter(typeof(AttributedSimpleObjectA)));

            Assert.False(haveFilter(typeof(BasicService)));
        }
        
        [Fact]
        public void TypesThat_Contains()
        {
            Func<Type, bool> haveFilter = TypesThat.Contains("Simple");

            Assert.True(haveFilter(typeof(AttributedSimpleObjectA)));

            Assert.False(haveFilter(typeof(BasicService)));
        }

        [Fact]
        public void TypesThat_StartWith_Not()
        {
            Func<Type, bool> haveFilter = TypesThat.Not().StartWith("A");

            Assert.False(haveFilter(typeof(AttributedSimpleObjectA)));

            Assert.True(haveFilter(typeof(BasicService)));
        }
    }
}
