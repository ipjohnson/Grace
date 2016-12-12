using System;
using Grace.Utilities;
using Xunit;

namespace Grace.Tests.Utilities
{
    public class GenericFilterGroupTests
    {
        [Fact]
        public void GenericFilterGroup_Null_Reference()
        {
            Assert.Throws<ArgumentNullException>(() => new GenericFilterGroup<int>(null));
        }
    }
}
