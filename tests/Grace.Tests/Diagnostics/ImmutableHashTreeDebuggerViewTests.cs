using System.Linq;
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
            Assert.Single(keys);
            Assert.Equal(5, keys[0]);

            var values = debugger.Values.ToArray();
            Assert.Single(values);
            Assert.Equal("Hello", values[0]);
        }
        
        [Fact]
        public void ImmutableHashTreeDebuggerView_Items()
        {
            var tree = ImmutableHashTree<int, string>.Empty.Add(5, "Hello");

            var debugger = new ImmutableHashTreeDebuggerView<int, string>(tree);

            var items = debugger.Items.ToArray();

            Assert.Single(items);
            Assert.Equal(5, items[0].Key);
            Assert.Equal("Hello", items[0].Value);
            Assert.Equal("5", items[0].DebuggerNameDisplayString);
            Assert.Equal("Hello", items[0].DebuggerDisplayString);
        }
    }
}
