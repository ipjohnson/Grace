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

            Assert.Throws<ArgumentNullException>(() => ImmutableLinkedList.ThreadSafeAdd(ref value,2));

            Assert.Throws<ArgumentNullException>(() => ImmutableLinkedList.ThreadSafeEmpty(ref value));

            Assert.Throws<ArgumentNullException>(() => ImmutableLinkedList.ThreadSafeRemove(ref value,5));

            Assert.Throws<ArgumentNullException>(() => ImmutableLinkedList.ThreadSafeAddRange(ref value, new []{5}));

            value = ImmutableLinkedList<int>.Empty;
            
            Assert.Throws<ArgumentNullException>(() => ImmutableLinkedList.ThreadSafeAddRange(ref value, null));
        }

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

        private ImmutableLinkedList<int> _linkedList;
        private List<int> _finalList;
        private ManualResetEvent _startEvent;
        private CountdownEvent _countdownEvent;
        private int _addAmount = 50000;

        [Fact]
        public void ImmutableLinkedList_Multithreaded_Test()
        {
            int writerCount = 4;

            _finalList = new List<int>();
            _linkedList = ImmutableLinkedList<int>.Empty;
            _countdownEvent = new CountdownEvent(4);
            _startEvent = new ManualResetEvent(false);

            var listOfTasks = new List<Task>();

            for (int i = 0; i < writerCount; i++)
            {
                var value = i;
                var task = Task.Run(() => AddRangeToList(value * _addAmount));

                listOfTasks.Add(task);
            }

            listOfTasks.Add(Task.Run(() => RemoveFromList()));

            _startEvent.Set();

            Task.WaitAll(listOfTasks.ToArray(), 60 * 1000);

            _finalList.Sort();

            Assert.Equal(_finalList.Count, _addAmount * writerCount);

            for (int i = 0; i < (_addAmount * writerCount); i++)
            {
                Assert.Equal(i, _finalList[i]);
            }
        }

        private void RemoveFromList()
        {
            while (!_countdownEvent.WaitHandle.WaitOne(0))
            {
                for (int i = 0; i < 500; i++)
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

            for (int i = startValue; i < (startValue + _addAmount); i++)
            {
                ImmutableLinkedList.ThreadSafeAdd(ref _linkedList, i);

                if (_addAmount % 1000 == 0)
                {
                    Thread.Sleep(0);
                }
            }

            _countdownEvent.Signal();
        }
    }
}
