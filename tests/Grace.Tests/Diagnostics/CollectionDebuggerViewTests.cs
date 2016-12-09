using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grace.Diagnostics;
using Xunit;

namespace Grace.Tests.Diagnostics
{
    public class CollectionDebuggerViewTests
    {
        [Fact]
        public void CollectionDebuggerView_Items()
        {
            var debugger = new CollectionDebuggerView<int>(new[] { 1, 2, 3 });

            var array = debugger.Items.ToArray();

            Assert.Equal(3, array.Length);
            Assert.Equal(1, array[0]);
            Assert.Equal(2, array[1]);
            Assert.Equal(3, array[2]);
        }

        [Fact]
        public void CollectionDebuggerView_DebuggerDisplayValue()
        {
            var debugger = new CollectionDebuggerView<int>(new[] { 1, 2, 3 });

            var property = 
                debugger.GetType().GetTypeInfo().GetProperty("DebuggerDisplayValue", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.Equal("Count: 3",property.GetValue(debugger));
        }
    }
}
