﻿using System.Reflection;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.AttributeTests
{
    public class KeyedAttributeTests
    {
        [Fact]
        public void KeyedAttributeTest()
        {
            var container = new DependencyInjectionContainer();

            var assembly = Assembly.GetAssembly(typeof(ImportKeyedMultiple));

            container.Configure(c => c
                .ExportAssembly(assembly).ExportAttributedTypes().Where(TypesThat.AreInTheSameNamespaceAs(typeof(ImportKeyedMultiple)))
            );

            var instance = container.Locate<ImportKeyedMultiple>();

            Assert.NotNull(instance);
            Assert.Equal("A", instance.ServiceA.SomeMethod());
            Assert.Equal("B", instance.ServiceB.SomeMethod());
        }
    }
}
