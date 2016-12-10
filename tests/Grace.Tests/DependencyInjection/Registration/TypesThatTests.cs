using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Attributes;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
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
            Func<Type, bool> sameNamespace = TypesThat.AreInTheSameNamespaceAs(typeof(DependencyInjectionContainer),
                true);

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
                x => ((SomeTestAttribute) x).TestValue == 5);

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
        public void ComplexHaveAttribute()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(TypesThatTests).GetTypeInfo().Assembly.ExportedTypes).
                ByInterface(typeof(IAttributedSimpleObject)).
                Where(TypesThat.HaveAttribute<SomeTestAttribute>()));

            IEnumerable<IAttributedSimpleObject> simpleObjects = container.LocateAll<IAttributedSimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(3, simpleObjects.Count());
        }

        [Fact]
        public void ComplexHaveAttributeFiltered()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(TypesThatTests).GetTypeInfo().Assembly.ExportedTypes).
                ByInterface(typeof(IAttributedSimpleObject)).
                Where(TypesThat.HaveAttribute<SomeTestAttribute>(x => x.TestValue == 5)));

            IEnumerable<IAttributedSimpleObject> simpleObjects = container.LocateAll<IAttributedSimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(1, simpleObjects.Count());
        }


        [Fact]
        public void ComplexHaveAttributeNonGeneric()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(TypesThatTests).GetTypeInfo().Assembly.ExportedTypes).
                ByInterface(typeof(IAttributedSimpleObject)).
                Where(TypesThat.HaveAttribute(t => t == typeof(SomeTestAttribute))));

            IEnumerable<IAttributedSimpleObject> simpleObjects = container.LocateAll<IAttributedSimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(3, simpleObjects.Count());
        }

        private class PrivateClass
        {
            
        }

        [Fact]
        public void TypesThat_AreNotPublic()
        {
            var testFunc = (Func<Type, bool>) TypesThat.AreNotPublic();

            Assert.True(testFunc(typeof(PrivateClass)));
        }

        [Fact]
        public void TypesThat_AreGeneric()
        {
            var testFunc = (Func<Type, bool>) TypesThat.AreConstructedGeneric();

            Assert.True(testFunc(typeof(DependentService<IBasicService>)));
        }

        [Fact]
        public void TypesThat_AreOpenGeneric()
        {
            var testFunc = (Func<Type, bool>)TypesThat.AreOpenGeneric();

            Assert.True(testFunc(typeof(DependentService<>)));
        }

        [Fact]
        public void TypesThat_ArePublic()
        {
            var testFunc = (Func<Type, bool>)TypesThat.ArePublic();

            Assert.True(testFunc(typeof(DependentService<>)));
            Assert.False(testFunc(typeof(PrivateClass)));
        }

        [Fact]
        public void TypesThat_Match()
        {
            var testFunc = (Func<Type, bool>) TypesThat.Match(t => t == typeof(DependentService<>));

            Assert.True(testFunc(typeof(DependentService<>)));
            Assert.False(testFunc(typeof(TypesThatTests)));
        }

        public class PropertyClass
        {
            public int PropertyA { get; set; }
        }

        [Fact]
        public void TypesThat_HaveProperty()
        {
            var testFunc = (Func<Type, bool>) TypesThat.HaveProperty("PropertyA");

            Assert.True(testFunc(typeof(PropertyClass)));
        }

        [Fact]
        public void TypesThat_HaveProperty_Generic()
        {
            var testFunc = (Func<Type, bool>)TypesThat.HaveProperty<int>();

            Assert.True(testFunc(typeof(PropertyClass)));
        }


        [Fact]
        public void TypesThat_HaveProperty_Generic_With_Name()
        {
            var testFunc = (Func<Type, bool>)TypesThat.HaveProperty<int>("PropertyA");

            Assert.True(testFunc(typeof(PropertyClass)));
        }
    }
}