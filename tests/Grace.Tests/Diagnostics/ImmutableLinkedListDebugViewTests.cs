using Grace.Data.Immutable;
using Grace.Diagnostics;
using Xunit;

namespace Grace.Tests.Diagnostics
{
    public class ImmutableLinkedListDebugViewTests
    {
        [Fact]
        public void ImmutableLinkedListDebugView_Test()
        {
            var debugger = new ImmutableLinkedListDebugView<int>(ImmutableLinkedList<int>.Empty.Add(5));

            Assert.Equal(1, debugger.Items.Length);
            Assert.Equal(5, debugger.Items[0]);
        }
    }
}
