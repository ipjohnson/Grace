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
    }
}
