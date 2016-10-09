using System.Collections.Generic;
using Grace.Data.Immutable;
using Xunit;

namespace Grace.Tests.Data.Immutable
{
    public class ImmutableLinkedListTests
    {
        [Fact]
        public void ImmutableLinkedList_Add_Test()
        {
            var list = ImmutableLinkedList<int>.Empty;

            list = list.Add(5);

            Assert.Equal(1, list.Count);
        }

        [Fact]
        public void ImmutableLinkedList_Enumerator_Test()
        {
            var list = ImmutableLinkedList<int>.Empty;

            list = list.Add(5);
            list = list.Add(10);
            list = list.Add(15);

            var newList = new List<int>(list);

            Assert.Equal(3, newList.Count);
            Assert.True(newList.Contains(5));
            Assert.True(newList.Contains(10));
            Assert.True(newList.Contains(15));
        }
    }
}
