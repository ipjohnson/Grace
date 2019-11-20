using Grace.Data;
using Xunit;

namespace Grace.Tests.Data
{
    public class ReflectionServiceTests
    {
        [Fact]
        public void GetPropertiesFromObject()
        {
            var values = ReflectionService.GetPropertiesFromObject(new { Test = "Property", Value = 4 });

            Assert.Equal(2, values.Count);
            Assert.Equal("Property", values["Test"]);
            Assert.Equal(4, values["Value"]);
        }

        public class BaseClass
        {
            public int PropOne => 1;
        }

        public class InheritTestClass : BaseClass
        {
            public int PropTwo => 2;
        }

        [Fact]
        public void GetPropertiesInheritClass()
        {
            var values = ReflectionService.GetPropertiesFromObject(new InheritTestClass());

            Assert.Equal(2, values.Count);
            Assert.Equal(1, values["PropOne"]);
            Assert.Equal(2, values["PropTwo"]);
        }

    }
}
