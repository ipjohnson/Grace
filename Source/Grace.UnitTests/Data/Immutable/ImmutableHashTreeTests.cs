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

        [Fact]
        public void IterateInOrderAction()
        {
            ImmutableHashTree<string, Type> allTypes = ImmutableHashTree<string, Type>.Empty;

            foreach (Type exportedType in typeof(int).Assembly.ExportedTypes)
            {
                allTypes = allTypes.Add(exportedType.FullName, exportedType);
            }

            List<Type> types = new List<Type>();

            allTypes.IterateInOrder((k, v) => types.Add(v));

            int differenceCount = typeof(int).Assembly.ExportedTypes.Except(types).Count();

            Assert.Equal(0, differenceCount);
        }

        [Fact]
        public void IterateInOrderEnumerable()
        {
            ImmutableHashTree<string, Type> allTypes = ImmutableHashTree<string, Type>.Empty;

            foreach (Type exportedType in typeof(int).Assembly.ExportedTypes)
            {
                allTypes = allTypes.Add(exportedType.FullName, exportedType);
            }

            IEnumerable<Type> types = allTypes.IterateInOrder().Select(x => x.Value);

            int differenceCount = typeof(int).Assembly.ExportedTypes.Except(types).Count();

            Assert.Equal(0, differenceCount);
        }
    }
}
