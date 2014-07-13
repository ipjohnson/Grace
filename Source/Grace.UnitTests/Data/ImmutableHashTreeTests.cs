using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Grace.Data;
using Xunit;

namespace Grace.UnitTests.Data
{
    public class ImmutableHashTreeTests
    {
        [Fact]
        public void ImmutableHashTreeTest()
        {
            ImmutableHashTree<string, Type> hashTree = LoadWithTypes();

            foreach (Type exportedType in typeof(int).Assembly.ExportedTypes)
            {
                string typeName = exportedType.FullName;

                Type found = hashTree.Search(typeName);

                Assert.NotNull(found);
            }
        }

        [Fact]
        public void ImmutableHashTreeKeysTest()
        {
            ImmutableHashTree<string, Type> hashTree = LoadWithTypes();

            List<string> keys = hashTree.Keys();

            IEnumerable<string> differences = typeof(int).Assembly.ExportedTypes.Select(x => x.FullName).Except(keys);

            Assert.Equal(0, differences.Count());
        }

        [Fact]
        public void ImmutableHashTreeValuesTest()
        {
            ImmutableHashTree<string, Type> hashTree = LoadWithTypes();

            List<Type> types = hashTree.Values();

            IEnumerable<Type> differences = typeof(int).Assembly.ExportedTypes.Except(types);

            Assert.Equal(0, differences.Count());
        }

        [Fact]
        public void ImmutableHashTreeToListTest()
        {
            ImmutableHashTree<string, Type> hashTree = LoadWithTypes();

            List<KeyValuePair<string,Type>> values = hashTree.ToList();

            IEnumerable<KeyValuePair<string, Type>> selectedTypes =
                typeof(int).Assembly.ExportedTypes.Select(x => new KeyValuePair<string, Type>(x.FullName, x));

            IEnumerable<KeyValuePair<string, Type>> differences = selectedTypes.Except(values);

            Assert.Equal(0, differences.Count());
        }
        
        private ImmutableHashTree<string, Type> LoadWithTypes()
        {
            ImmutableHashTree<string, Type> returnValue = ImmutableHashTree<string, Type>.Empty;

            foreach (Type exportedType in typeof(int).Assembly.ExportedTypes)
            {
                returnValue = returnValue.Add(exportedType.FullName, exportedType);
            }

            return returnValue;
        }
    }
}
