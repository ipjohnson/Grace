using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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


    }
}
