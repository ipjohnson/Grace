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
            var values = ReflectionService.GetPropertiesFromObject(new { Test = "Property", Value = 4 });

            Assert.Equal(2, values.Count);
            Assert.Equal("Property", values["Test"]);
            Assert.Equal(4, values["Value"]);            
        }
    }
}
