using System.Collections.Generic;
using System.Linq;

namespace Grace.UnitTests.Classes.Simple
{
	public class ImportObservableCollectionService
	{
		public ImportObservableCollectionService(IEnumerable<ISimpleObject> simpleObjects)
		{
			Count = simpleObjects.Count();
		}

		public int Count { get; private set; }
	}
}