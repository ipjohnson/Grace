using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.Data.Immutable;
using Xunit;

namespace Grace.Tests.Data.Immutable
{
    public class ImmutableArrayTests
    {
        [Fact]
        public void ImmutableArray_Null_Reference_Check()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArray.From<int>(null));
        }
        
        [Fact]
        public void ImmutableArray_Create()
        {
            var array = ImmutableArray.Create(5, 10, 15);

            var newList = new List<int>(array);

            Assert.Equal(3, newList.Count);
            Assert.True(newList.Contains(5));
            Assert.True(newList.Contains(10));
            Assert.True(newList.Contains(15));
        }

        [Fact]
        public void ImmutableArray_From()
        {
            var array = ImmutableArray.From(new List<int>{ 5, 10, 15});

            var newList = new List<int>(array);

            Assert.Equal(3, newList.Count);
            Assert.True(newList.Contains(5));
            Assert.True(newList.Contains(10));
            Assert.True(newList.Contains(15));
        }

        [Fact]
        public void ImmutableArray_Contains()
        {
            var array = ImmutableArray.Create(5, 10, 15);

            Assert.True(array.Contains(5));
            Assert.True(array.Contains(10));
            Assert.True(array.Contains(15));
            Assert.False(array.Contains(0));
        }


        [Fact]
        public void ImmutableArray_AddRange()
        {
            var array = ImmutableArray<int>.Empty;

            array = array.AddRange(new List<int> {5, 10, 15});

            Assert.True(array.Contains(5));
            Assert.True(array.Contains(10));
            Assert.True(array.Contains(15));
            Assert.False(array.Contains(0));
        }
    }
}
