using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.Data.Immutable;
using Xunit;

namespace Grace.UnitTests.Data.Immutable
{
    public class ImmutableHashTreeTests
    {
        [Fact]
        public void BulkLoadHashTree()
        {
            ImmutableHashTree<string, Type> allTypes = ImmutableHashTree<string, Type>.Empty;

            foreach (Type exportedType in typeof(int).Assembly.ExportedTypes)
            {
                allTypes = allTypes.Add(exportedType.FullName, exportedType);
            }

            foreach (Type exportedType in typeof(int).Assembly.ExportedTypes)
            {
                Type outType;

                Assert.True(allTypes.TryGetValue(exportedType.FullName, out outType));
            }
        }
    }
}
