using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.Data.Immutable;
using Grace.Diagnostics;
using Xunit;

namespace Grace.Tests.Diagnostics
{
    public class ImmutableHashTreeDebuggerViewTests
    {
        [Fact]
        public void ImmutableHashTreeDebuggerView_Keys_And_Values()
        {
            var tree = ImmutableHashTree<int, string>.Empty.Add(5, "Hello");

            var debugger = new ImmutableHashTreeDebuggerView<int, string>(tree);

            var keys = debugger.Keys.ToArray();
            Assert.Equal(1, keys.Length);
            Assert.Equal(5, keys[0]);

            var values = debugger.Values.ToArray();
            Assert.Equal(1, values.Length);
            Assert.Equal("Hello", values[0]);
        }
        
        [Fact]
        public void ImmutableHashTreeDebuggerView_Items()
        {
            var tree = ImmutableHashTree<int, string>.Empty.Add(5, "Hello");

            var debugger = new ImmutableHashTreeDebuggerView<int, string>(tree);

            var items = debugger.Items.ToArray();

            Assert.Equal(1, items.Length);
            Assert.Equal(5, items[0].Key);
            Assert.Equal("Hello", items[0].Value);
            Assert.Equal("5", items[0].DebuggerNameDisplayString);
            Assert.Equal("Hello", items[0].DebuggerDisplayString);
        }
    }
}
