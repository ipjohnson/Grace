using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Attributes;
using Xunit;

namespace Grace.Tests.DependencyInjection.AttributeTests
{
    public class PriorityAttributeTests
    {
        [Fact]
        public void Export_PriorityAttribute()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportAssemblyContaining<ExportAttributeTests>().
                ExportAttributedTypes().
                Where(TypesThat.AreInTheSameNamespaceAs<TestAttribute>()));


            var serviceList = container.LocateAll<IPriorityAttributeService>();

            Assert.NotNull(serviceList);
            Assert.Equal(5, serviceList.Count);

            Assert.IsType<PriorityAttributeServiceE>(serviceList[0]);
            Assert.IsType<PriorityAttributeServiceD>(serviceList[1]);
            Assert.IsType<PriorityAttributeServiceC>(serviceList[2]);
            Assert.IsType<PriorityAttributeServiceB>(serviceList[3]);
            Assert.IsType<PriorityAttributeServiceA>(serviceList[4]);
        }
    }
}
