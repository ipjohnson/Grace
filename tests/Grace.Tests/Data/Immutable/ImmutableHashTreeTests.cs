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

        private ImmutableHashTree<int, int> _hashTree;
        private ManualResetEvent _startEvent;
        private int _addAmount = 50000;

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
    }
}
