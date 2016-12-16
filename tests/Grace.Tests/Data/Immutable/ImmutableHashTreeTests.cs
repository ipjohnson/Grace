using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grace.Data.Immutable;
using Xunit;

namespace Grace.Tests.Data.Immutable
{
    public class ImmutableHashTreeTests
    {
        [Fact]
        public void ImmutableHashTree_Null_Reference_Check()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableHashTree<int?, int>.Empty.Add(null, 5));
            Assert.Throws<ArgumentNullException>(() => ImmutableHashTree<int?, int>.Empty.ContainsKey(null));
        }

        [Fact]
        public void ImmutableHashTree_ThreadsafeAdd_Null_Tree()
        {
            ImmutableHashTree<int, int> tree = null;

            Assert.Throws<ArgumentNullException>(() => ImmutableHashTree.ThreadSafeAdd(ref tree, 10, 10));
        }

        [Fact]
        public void ImmutableHashTree_ThreadsafeAdd_Null_Key()
        {
            ImmutableHashTree<int?, int> tree = ImmutableHashTree<int?, int>.Empty;

            Assert.Throws<ArgumentNullException>(() => ImmutableHashTree.ThreadSafeAdd(ref tree, null, 10));
        }


        [Fact]
        public void ImmutableHashTree_TryGet_Null_Key()
        {
            ImmutableHashTree<int?, int> tree = ImmutableHashTree<int?, int>.Empty;

            int value;

            Assert.Throws<ArgumentNullException>(() => tree.TryGetValue(null, out value));
        }

        [Fact]
        public void ImmutableHashTree_GetValue_Null_Key()
        {
            ImmutableHashTree<int?, int> tree = ImmutableHashTree<int?, int>.Empty;

            Assert.Throws<ArgumentNullException>(() => tree.GetValueOrDefault(null));
        }

        [Fact]
        public void ImmutableHashTree_Index_Throws_KeyNotFound()
        {
            ImmutableHashTree<int?, int> tree = ImmutableHashTree<int?, int>.Empty;

            Assert.Throws<KeyNotFoundException>(() => tree[5]);
        }

        [Fact]
        public void ImmutableHashTree_IterateInOrder_Null_Throws()
        {
            ImmutableHashTree<int?, int> tree = ImmutableHashTree<int?, int>.Empty;

            Assert.Throws<ArgumentNullException>(() => tree.IterateInOrder(null));
        }

        [Fact]
        public void ImmutableHashTree_From()
        {
            var dictionary = new Dictionary<int, int> { { 5, 5 }, { 10, 10 }, { 15, 15 } };
            var tree = ImmutableHashTree.From(dictionary);

            Assert.Equal(3, tree.Count);

            Assert.Equal(5, tree[5]);
            Assert.Equal(10, tree[10]);
            Assert.Equal(15, tree[15]);
        }

        [Fact]
        public void ImmutableHashTree_Throws_Key_Already_Exists()
        {
            var tree = ImmutableHashTree<int, int>.Empty.Add(10, 10);

            Assert.Throws<KeyExistsException<int>>(() => tree.Add(10, 10));
        }

        [Fact]
        public void ImmutableHashTree_TryGetValue_ReturnsValue()
        {
            var tree = ImmutableHashTree<int, int>.Empty.Add(10, 10);

            int testValue;
            Assert.False(tree.TryGetValue(5,out testValue));
            Assert.Equal(0,testValue);
        }
        #region Thread test

        private ImmutableHashTree<int, int> _hashTree;
        private ManualResetEvent _startEvent;
        private int _addAmount = 10000;

        [Fact]
        public void ImmutableHashTree_Threading_Test()
        {
            _startEvent = new ManualResetEvent(false);
            _hashTree = ImmutableHashTree<int, int>.Empty;

            int threadCount = 4;

            var tasks = new List<Task>();

            for (int i = 0; i < threadCount; i++)
            {
                var value = i;

                tasks.Add(Task.Run(() => AddRangeToTree(value * _addAmount)));
            }

            _startEvent.Set();

            Task.WaitAll(tasks.ToArray(), 60 * 1000);

            List<int> values = new List<int>();

            _hashTree.IterateInOrder((key, value) => values.Add(key));

            values.Sort();

            Assert.Equal(_addAmount * threadCount, values.Count);

            for (int i = 0; i < (_addAmount * threadCount); i++)
            {
                Assert.Equal(i, values[i]);
            }
        }

        private void AddRangeToTree(int startValue)
        {
            _startEvent.WaitOne();

            for (int i = startValue; i < (_addAmount + startValue); i++)
            {
                ImmutableHashTree.ThreadSafeAdd(ref _hashTree, i, i);

                if (i % 1000 == 0)
                {
                    Thread.Sleep(0);
                }
            }
        }
        #endregion
    }
}
