﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            Assert.Throws<Exception>(() => service.GetAttributes(new BasicService()));
        }
    }
}
