using System;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.AttributeTests
{
    public class AttributeDiscoveryServiceTests
    {
        [Fact]
        public void AttributeDiscoveryService_Null_Returns_Empty()
        {
            var service = new AttributeDiscoveryService();

            Assert.Empty(service.GetAttributes(null));
        }

        [Fact]
        public void AttributeDiscoveryService_Throws_Exception()
        {
            var service = new AttributeDiscoveryService();

            Assert.Throws<NotSupportedException>(() => service.GetAttributes(new BasicService()));
        }
    }
}
