using System;
using System.Collections;
using System.Collections.Generic;
using Grace.Data.Immutable;
using Xunit;

namespace Grace.Tests.Data.Immutable
{
    public class ImmutableArrayTests
    {
        [Fact]
        public void ImmutableArray_IsReadOnly()
        {
            Assert.True(ImmutableArray<int>.Empty.IsReadOnly);
        }

        [Fact]
        public void ImmutableArray_Null_Reference_Check()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArray.From<int>(null));
        }

        [Fact]
        public void ImmutableArray_IndexOf_Found()
        {
            var array = ImmutableArray.Create(5, 10, 15);

            Assert.Equal(1, array.IndexOf(10));
        }

        [Fact]
        public void ImmutableArray_IndexOf_Not_Found()
        {
            var array = ImmutableArray.Create(5, 10, 15);

            Assert.Equal(-1, array.IndexOf(20));
        }

        [Fact]
        public void ImmutableArray_Insert_Beginning()
        {
            var array = ImmutableArray.Create(5, 10, 15);

            array = array.Insert(0, 1);

            Assert.Equal(1, array[0]);
            Assert.Equal(5, array[1]);
            Assert.Equal(10, array[2]);
            Assert.Equal(15, array[3]);
        }

        [Fact]
        public void ImmutableArray_Insert_End()
        {
            var array = ImmutableArray.Create(5, 10, 15);

            array = array.Insert(3, 20);

            Assert.Equal(5, array[0]);
            Assert.Equal(10, array[1]);
            Assert.Equal(15, array[2]);
            Assert.Equal(20, array[3]);
        }

        [Fact]
        public void ImmutableArray_Insert_Middle()
        {
            var array = ImmutableArray.Create(5, 10, 15);

            array = array.Insert(2, 20);

            Assert.Equal(5, array[0]);
            Assert.Equal(10, array[1]);
            Assert.Equal(20, array[2]);
            Assert.Equal(15, array[3]);
        }

        [Fact]
        public void ImmutableArray_Compare()
        {
            var array = ImmutableArray.Create(5, 10, 15);
            var array2 = ImmutableArray.Create(5, 10, 15);
            var array3 = ImmutableArray.Create(4, 8, 12);

            var comparer = (IStructuralComparable)array;

            Assert.Equal(0, comparer.CompareTo(array2, new CustomComparer()));
            Assert.Equal(0, comparer.CompareTo(new[] { 5, 10, 15 }, new CustomComparer()));
            Assert.NotEqual(0, comparer.CompareTo(array3, new CustomComparer()));
        }

        [Fact]
        public void ImmutableArray_FromArray()
        {
            var array = (ImmutableArray<int>)new[] { 5, 10, 15 };

            Assert.Equal(5, array[0]);
            Assert.Equal(10, array[1]);
            Assert.Equal(15, array[2]);
        }

        [Fact]
        public void ImmutableArray_CopyTo_Array()
        {
            var array = ImmutableArray.Create(5, 10, 15);

            var copyArray = new int[3];

            array.CopyTo(copyArray, 0);

            Assert.Equal(5, copyArray[0]);
            Assert.Equal(10, copyArray[1]);
            Assert.Equal(15, copyArray[2]);
        }

        [Fact]
        public void ImmutableArray_Equal()
        {
            var array = ImmutableArray.Create(5, 10, 15);
            var array2 = ImmutableArray.Create(5, 10, 15);
            var array3 = ImmutableArray.Create(4, 8, 12);

            Assert.Equal(array, array2);
            Assert.NotEqual(array, array3);
        }

        [Fact]
        public void ImmutableArray_Equal_Symbol()
        {
            var array = ImmutableArray.Create(5, 10, 15);
            var array2 = ImmutableArray.Create(5, 10, 15);
            var array3 = ImmutableArray.Create(4, 8, 12);

            Assert.True(array == array2);
            Assert.True(array != array3);
        }

        [Fact]
        public void ImmutableArray_Nullable_Equal_Symbol()
        {
            ImmutableArray<int>? array = ImmutableArray.Create(5, 10, 15);
            ImmutableArray<int>? array2 = ImmutableArray.Create(5, 10, 15);
            ImmutableArray<int>? array3 = ImmutableArray.Create(4, 8, 12);

            Assert.True(array == array2);
            Assert.True(array != array3);
        }

        public class CustomComparer : IEqualityComparer, IComparer
        {
            public bool Equals(object x, object y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(object obj)
            {
                return obj.GetHashCode();
            }

            public int Compare(object x, object y)
            {
                return Comparer<int>.Default.Compare((int)x, (int)y);
            }
        }


        [Fact]
        public void ImmutableArray_StructuralEqual()
        {
            var array = ImmutableArray.Create(5, 10, 15);
            var array2 = ImmutableArray.Create(5, 10, 15);

            var stArray1 = ((IStructuralEquatable)array);

            Assert.True(stArray1.Equals(array2, new CustomComparer()));
            Assert.True(stArray1.Equals(new[] { 5, 10, 15 }, new CustomComparer()));
        }

        [Fact]
        public void ImmutableArray_GetHashCode()
        {
            var array = ImmutableArray.Create(5, 10, 15);

            Assert.NotEqual(0, array.GetHashCode());
        }

        [Fact]
        public void ImmutableArray_GetHashCode_StructuralyEqual()
        {
            var array = ImmutableArray.Create(5, 10, 15);

            Assert.NotEqual(0, ((IStructuralEquatable)array).GetHashCode(new CustomComparer()));
        }

        [Fact]
        public void ImmutableArray_Create()
        {
            var array = ImmutableArray.Create(5, 10, 15);

            var newList = new List<int>(array);

            Assert.Equal(3, newList.Count);
            Assert.Contains(5, newList);
            Assert.Contains(10, newList);
            Assert.Contains(15, newList);
        }

        [Fact]
        public void ImmutableArray_From()
        {
            var array = ImmutableArray.From(new List<int> { 5, 10, 15 });

            var newList = new List<int>(array);

            Assert.Equal(3, newList.Count);
            Assert.Contains(5, newList);
            Assert.Contains(10, newList);
            Assert.Contains(15, newList);
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

            array = array.AddRange(new List<int> { 5, 10, 15 });

            Assert.True(array.Contains(5));
            Assert.True(array.Contains(10));
            Assert.True(array.Contains(15));
            Assert.False(array.Contains(0));
        }
    }
}
