using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grace.Data.Immutable;
using Xunit;

namespace Grace.Tests.Data.Immutable
{
    public class ImmutableLinkedListTests
    {
        [Fact]
        public void ImmutableLinkedList_Null_Reference_Check()
        {
            var value = (ImmutableLinkedList<int>) null;
            
            Assert.Throws<ArgumentNullException>(() => ImmutableLinkedList.ThreadSafeAddRange(ref value, new []{5}));

            value = ImmutableLinkedList<int>.Empty;
            
            Assert.Throws<ArgumentNullException>(() => ImmutableLinkedList.ThreadSafeAddRange(ref value, null));

            Assert.Throws<ArgumentNullException>(() => ImmutableLinkedList<int>.Empty.Visit(null));

            Assert.Throws<ArgumentNullException>(() => ImmutableLinkedList<int>.Empty.AddRange(null));

            Assert.Throws<ArgumentNullException>(() => ImmutableLinkedList.From<int>(null));
        }

        [Fact]
        public void ImmutableLinkedList_Create_Returns_Empty_For_Null()
        {
            Assert.Equal(ImmutableLinkedList<int>.Empty, ImmutableLinkedList.Create<int>(null));
            Assert.Equal(ImmutableLinkedList<int>.Empty, ImmutableLinkedList.Create(new int[0]));
        }

        [Fact]
        public void ImmutableLinkedList_Add_Test()
        {
            var list = ImmutableLinkedList<int>.Empty;

            list = list.Add(5);

            Assert.Single(list);
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
            Assert.Contains(5, newList);
            Assert.Contains(10, newList);
            Assert.Contains(15, newList);
        }

        [Fact]
        public void ImmutableLinkedList_From()
        {
            var list = ImmutableLinkedList.From(new[] { 5, 10, 15 });

            var newList = new List<int>(list);

            Assert.Equal(3, newList.Count);
            Assert.Contains(5, newList);
            Assert.Contains(10, newList);
            Assert.Contains(15, newList);
        }

        [Fact]
        public void ImmutableLinkedList_Create()
        {
            var list = ImmutableLinkedList.Create(5, 10, 15);

            var newList = new List<int>(list);

            Assert.Equal(3, newList.Count);
            Assert.Contains(5, newList);
            Assert.Contains(10, newList);
            Assert.Contains(15, newList);
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
            Assert.Contains(5, newList);
            Assert.Contains(10, newList);
            Assert.Contains(15, newList);
        }
        
        [Fact]
        public void ImmutableLinkedList_ThreadSafeEmpty()
        {
            var list = ImmutableLinkedList.Create(5, 10, 15);

            var newList = new List<int>(list);

            Assert.Equal(3, newList.Count);
            Assert.Contains(5, newList);
            Assert.Contains(10, newList);
            Assert.Contains(15, newList);

            ImmutableLinkedList.ThreadSafeEmpty(ref list);

            newList = new List<int>(list);

            Assert.Empty(newList);
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
        public void ImmutableLinkedList_ReadOnlyList()
        {
            var list = ImmutableLinkedList.Create(5, 10, 15);

            Assert.Equal(15, list[0]);
            Assert.Equal(10, list[1]);
            Assert.Equal(5, list[2]);
        }

        [Fact]
        public void ImmutableLinkedList_ReadOnlyList_IndexException()
        {
            var list = ImmutableLinkedList.Create(5, 10, 15);

            Assert.Throws<ArgumentOutOfRangeException>(() => list[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[3]);
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

        private ImmutableLinkedList<int> _linkedList;
        private List<int> _finalList;
        private ManualResetEvent _startEvent;
        private CountdownEvent _countdownEvent;

        private const int AddAmount = 20_000;

        [Fact]
        public void ImmutableLinkedList_Multithreaded_Test()
        {
            var writerCount = 4;

            _finalList = new List<int>();
            _linkedList = ImmutableLinkedList<int>.Empty;
            _countdownEvent = new CountdownEvent(4);
            _startEvent = new ManualResetEvent(false);

            var listOfTasks = new List<Task>();

            for (var i = 0; i < writerCount; i++)
            {
                var value = i;
                var task = Task.Run(() => AddRangeToList(value * AddAmount));

                listOfTasks.Add(task);
            }

            listOfTasks.Add(Task.Run(() => RemoveFromList()));

            _startEvent.Set();

            Task.WaitAll(listOfTasks.ToArray(), 60 * 1_000);

            _finalList.Sort();

            Assert.Equal(_finalList.Count, AddAmount * writerCount);

            for (var i = 0; i < (AddAmount * writerCount); i++)
            {
                Assert.Equal(i, _finalList[i]);
            }
        }

        private void RemoveFromList()
        {
            while (!_countdownEvent.WaitHandle.WaitOne(0))
            {
                for (var i = 0; i < 500; i++)
                {
                    var list = ImmutableLinkedList.ThreadSafeEmpty(ref _linkedList);

                    _finalList.AddRange(list);
                }
            }

            var lastList = ImmutableLinkedList.ThreadSafeEmpty(ref _linkedList);

            _finalList.AddRange(lastList);
        }

        private void AddRangeToList(int startValue)
        {
            _startEvent.WaitOne();

            for (var i = startValue; i < (startValue + AddAmount); i++)
            {
                ImmutableLinkedList.ThreadSafeAdd(ref _linkedList, i);

                if (AddAmount % 1_000 == 0)
                {
                    Thread.Sleep(0);
                }
            }

            _countdownEvent.Signal();
        }
    }
}
