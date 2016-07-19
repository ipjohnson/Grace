using Grace.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Grace.UnitTests.Data
{
    public class ReflectionServiceTests
    {
        [Fact]
        public void GetPropertiesFromObject()
        {
            var propertyObject = new { Test = "Property", Value = 4 };

            var values = ReflectionService.GetPropertiesFromObject(propertyObject);

            Assert.Equal(2, values.Count);
            Assert.Equal("Property", values[nameof(propertyObject.Test)]);
            Assert.Equal(4, values[nameof(propertyObject.Value)]);            
        }

        [Fact]
        public void GetPropertiesFromObjectUppercase()
        {
            var propertyObject = new { Test = "Property", Value = 4 };

            var values = ReflectionService.GetPropertiesFromObject(propertyObject, PropertyNameCasing.Uppercase);

            Assert.Equal(2, values.Count);
            Assert.Equal(propertyObject.Test, values[nameof(propertyObject.Test).ToUpper()]);
            Assert.Equal(propertyObject.Value, values[nameof(propertyObject.Value).ToUpper()]);
        }

        [Fact]
        public void GetPropertiesFromObjectLowercase()
        {
            var propertyObject = new { Test = "Property", Value = 4 };

            var values = ReflectionService.GetPropertiesFromObject(propertyObject, PropertyNameCasing.Lowercase);

            Assert.Equal(2, values.Count);
            Assert.Equal(propertyObject.Test, values[nameof(propertyObject.Test).ToLower()]);
            Assert.Equal(propertyObject.Value, values[nameof(propertyObject.Value).ToLower()]);
        }
    }
}
