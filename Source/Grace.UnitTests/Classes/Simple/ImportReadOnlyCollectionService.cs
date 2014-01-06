using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Grace.UnitTests.Classes.Simple
{
	public class ImportReadOnlyCollectionService
	{
		public ImportReadOnlyCollectionService(IReadOnlyCollection<ISimpleObject> simpleObjects)
		{
			Assert.NotNull(simpleObjects);
			Assert.Equal(5, simpleObjects.Count());
		}
	}
}