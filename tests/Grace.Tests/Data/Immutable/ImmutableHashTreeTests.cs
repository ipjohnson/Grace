using System;
using System.Collections.Generic;
using System.Linq;
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
            var tree = ImmutableHashTree<int?, int>.Empty;

            Assert.Throws<ArgumentNullException>(() => ImmutableHashTree.ThreadSafeAdd(ref tree, null, 10));
        }

        [Fact]
        public void ImmutableHashTree_TryGet_Null_Key()
        {
            var tree = ImmutableHashTree<int?, int>.Empty;

            Assert.Throws<ArgumentNullException>(() => tree.TryGetValue(null, out var value));
        }
        
        [Fact]
        public void ImmutableHashTree_Index_Throws_KeyNotFound()
        {
            var tree = ImmutableHashTree<int?, int>.Empty;

            Assert.Throws<KeyNotFoundException>(() => tree[5]);
        }

        [Fact]
        public void ImmutableHashTree_IterateInOrder_Null_Throws()
        {
            var tree = ImmutableHashTree<int?, int>.Empty;

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

            Assert.False(tree.TryGetValue(5, out var testValue));
            Assert.Equal(0, testValue);
        }

        [Fact]
        public void ImmutableHashTree_ThreadSafeAdd_UpdateIfExists()
        {
            var tree = ImmutableHashTree<string, int>.Empty;

            Assert.Equal(5, ImmutableHashTree.ThreadSafeAdd(ref tree, "Hello", 5));

            Assert.Single(tree);
            Assert.Equal(5, tree["Hello"]);

            Assert.Equal(5, ImmutableHashTree.ThreadSafeAdd(ref tree, "Hello", 10));

            Assert.Single(tree);
            Assert.Equal(5, tree["Hello"]);

            Assert.Equal(10, ImmutableHashTree.ThreadSafeAdd(ref tree, "Hello", 10, updateIfExists: true));

            Assert.Single(tree);
            Assert.Equal(10, tree["Hello"]);

        }

        #region conflict tests

        public class ConflictClass
        {
            public ConflictClass(int firstInt, int secondInt)
            {
                FirstInt = firstInt;
                SecondInt = secondInt;
            }

            public int FirstInt { get; }

            public int SecondInt { get; }

            public override bool Equals(object obj)
            {
                var conflictClass = obj as ConflictClass;

                if (conflictClass != null)
                {
                    return conflictClass.FirstInt == FirstInt && conflictClass.SecondInt == SecondInt;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return FirstInt;
            }
        }

        [Fact]
        public void ImmutableHashTree_Conflict_Index()
        {
            var tree = ImmutableHashTree<ConflictClass, int>.Empty;

            for (var i = 0; i < 1_000; i++)
            {
                tree = tree.Add(new ConflictClass(i % 5, i), i);
            }

            Assert.Equal(1_000, tree.Count);

            for (var i = 0; i < 1_000; i++)
            {
                var conflict = new ConflictClass(i % 5, i);

                Assert.Equal(i, tree[conflict]);
            }
        }

        [Fact]
        public void ImmutableHashTree_Conflict_GetValue()
        {
            var tree = ImmutableHashTree<ConflictClass, int>.Empty;

            for (var i = 0; i < 1_000; i++)
            {
                tree = tree.Add(new ConflictClass(i % 5, i), i);
            }

            Assert.Equal(1_000, tree.Count);

            for (var i = 0; i < 1_000; i++)
            {
                var conflict = new ConflictClass(i % 5, i);

                Assert.Equal(i, tree.GetValueOrDefault(conflict));
            }
        }


        [Fact]
        public void ImmutableHashTree_Conflict_GetValue_Default()
        {
            var tree = ImmutableHashTree<ConflictClass, int>.Empty;

            tree = tree.Add(new ConflictClass(5, 1), 6);

            var value = tree.GetValueOrDefault(new ConflictClass(5, 2), 10);

            Assert.Equal(10, value);
        }


        [Fact]
        public void ImmutableHashTree_Conflict_IterateInOrder()
        {
            var tree = ImmutableHashTree<ConflictClass, int>.Empty;

            for (var i = 0; i < 1_000; i++)
            {
                tree = tree.Add(new ConflictClass(i % 5, i), i);
            }

            Assert.Equal(1_000, tree.Count);

            var list = new List<int>();

            tree.IterateInOrder((k, v) => list.Add(v));

            Assert.Equal(1_000, list.Count);
        }

        [Fact]
        public void ImmutableHashTree_Conflict_IterateInOrder_Enumerable()
        {
            var tree = ImmutableHashTree<ConflictClass, int>.Empty;

            for (var i = 0; i < 1_000; i++)
            {
                tree = tree.Add(new ConflictClass(i % 5, i), i);
            }

            Assert.Equal(1_000, tree.Count);

            Assert.Equal(1_000, tree.IterateInOrder().Count());
        }

        #endregion

        #region Thread test

        private ImmutableHashTree<int, int> _hashTree;
        private ManualResetEvent _startEvent;

        private const int AddAmount = 10_000;

        [Fact]
        public void ImmutableHashTree_Threading_Test()
        {
            _startEvent = new ManualResetEvent(false);
            _hashTree = ImmutableHashTree<int, int>.Empty;

            var threadCount = 4;

            var tasks = new List<Task>();

            for (var i = 0; i < threadCount; i++)
            {
                var value = i;

                tasks.Add(Task.Run(() => AddRangeToTree(value * AddAmount)));
            }

            _startEvent.Set();

            Task.WaitAll(tasks.ToArray(), 60 * 1_000);

            var values = new List<int>();

            _hashTree.IterateInOrder((key, value) => values.Add(key));

            values.Sort();

            Assert.Equal(AddAmount * threadCount, values.Count);

            for (var i = 0; i < (AddAmount * threadCount); i++)
            {
                Assert.Equal(i, values[i]);
            }
        }

        private void AddRangeToTree(int startValue)
        {
            _startEvent.WaitOne();

            for (var i = startValue; i < (AddAmount + startValue); i++)
            {
                ImmutableHashTree.ThreadSafeAdd(ref _hashTree, i, i);

                if (i % 1_000 == 0)
                {
                    Thread.Sleep(0);
                }
            }
        }
        #endregion
    }
}
