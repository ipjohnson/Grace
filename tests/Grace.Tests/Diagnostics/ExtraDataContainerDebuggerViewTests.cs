using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grace.Data;
using Grace.Diagnostics;
using NSubstitute;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.Diagnostics
{

    [SubFixtureInitialize]
    public class ExtraDataContainerDebuggerViewTests
    {
        [Theory]
        [AutoData]
        public void ExtraDataContainerDebuggerView_Items(ExtraDataContainerDebuggerView debugger,
                                                         IExtraDataContainer extraDataContainer)
        {
            extraDataContainer.KeyValuePairs.Returns(new[]
            {
                new KeyValuePair<object, object>(2, "World"),
                new KeyValuePair<object, object>(1, "Hello"),
            });

            var array = debugger.Items.ToArray();

            Assert.Equal(2, array.Length);
            Assert.Equal(1, array[0].Key);
            Assert.Equal("Hello", array[0].Value);
        }

        [Theory]
        [AutoData]
        public void ExtraDataContainerDebuggerView_DisplayValue(ExtraDataContainerDebuggerView debugger,
                                                         IExtraDataContainer extraDataContainer)
        {
            extraDataContainer.KeyValuePairs.Returns(new[]
            {
                new KeyValuePair<object, object>(2, "World"),
                new KeyValuePair<object, object>(1, "Hello"),
            });

            var property = debugger.GetType().GetTypeInfo().GetProperty("DebuggerDisplayValue", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.Equal("Count: 2", property.GetValue(debugger));
        }
    }
}
