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

        [Fact]
        public void ImmutableLinkedList_From()
        {
            var list = ImmutableLinkedList.From(new[] { 5, 10, 15 });

            var newList = new List<int>(list);

            Assert.Equal(3, newList.Count);
            Assert.True(newList.Contains(5));
            Assert.True(newList.Contains(10));
            Assert.True(newList.Contains(15));
        }

        [Fact]
        public void ImmutableLinkedList_Create()
        {
            var list = ImmutableLinkedList.Create(5, 10, 15);

            var newList = new List<int>(list);

            Assert.Equal(3, newList.Count);
            Assert.True(newList.Contains(5));
            Assert.True(newList.Contains(10));
            Assert.True(newList.Contains(15));
        }

        [Fact]
        public void ImmutableLinkedList_Contains()
        {
            var list = ImmutableLinkedList.Create(5, 10, 15);

            Assert.True(list.Contains(5));
            Assert.True(list.Contains(10));
            Assert.True(list.Contains(15));
            Assert.False(list.Contains(0));
        }

        [Fact]
        public void ImmutableLinkedList_ThreadSafeAdd()
        {
            var list = ImmutableLinkedList<int>.Empty;

            ImmutableLinkedList.ThreadSafeAdd(ref list, 5);
            ImmutableLinkedList.ThreadSafeAdd(ref list, 10);
            ImmutableLinkedList.ThreadSafeAdd(ref list, 15);

            var newList = new List<int>(list);

            Assert.Equal(3, newList.Count);
            Assert.True(newList.Contains(5));
            Assert.True(newList.Contains(10));
            Assert.True(newList.Contains(15));
        }

        [Fact]
        public void ImmutableLinkedList_ThreadSafeRemove()
        {
            var list = ImmutableLinkedList.Create(5, 10, 15);

            var newList = new List<int>(list);

            Assert.Equal(3, newList.Count);
            Assert.True(newList.Contains(5));
            Assert.True(newList.Contains(10));
            Assert.True(newList.Contains(15));

            ImmutableLinkedList.ThreadSafeRemove(ref list, 10);

            newList = new List<int>(list);

            Assert.Equal(2, newList.Count);
            Assert.True(newList.Contains(5));
            Assert.False(newList.Contains(10));
            Assert.True(newList.Contains(15));
        }

        [Fact]
        public void ImmutableLinkedList_ThreadSafeEmpty()
        {
            var list = ImmutableLinkedList.Create(5, 10, 15);

            var newList = new List<int>(list);

            Assert.Equal(3, newList.Count);
            Assert.True(newList.Contains(5));
            Assert.True(newList.Contains(10));
            Assert.True(newList.Contains(15));

            ImmutableLinkedList.ThreadSafeEmpty(ref list);

            newList = new List<int>(list);

            Assert.Equal(0, newList.Count);
        }

        [Fact]
        public void ImmutableLinkedList_Visit_Start_Beginning()
        {
            var list = ImmutableLinkedList.Create(5, 10, 15);

            var newList = new List<int>();

            list.Visit(i => newList.Add(i));

            Assert.Equal(3, newList.Count);
            Assert.Equal(15, newList[0]);
            Assert.Equal(10, newList[1]);
            Assert.Equal(5, newList[2]);
        }


        [Fact]
        public void ImmutableLinkedList_Visit_Start_End()
        {
            var list = ImmutableLinkedList.Create(5, 10, 15);

            var newList = new List<int>();

            list.Visit(i => newList.Add(i), startAtEnd: true);

            Assert.Equal(3, newList.Count);
            Assert.Equal(5, newList[0]);
            Assert.Equal(10, newList[1]);
            Assert.Equal(15, newList[2]);
        }
    }
}
