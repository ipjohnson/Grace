using System.Collections.Generic;
using System.Linq;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IImportIEnumerableService
	{
		int Count { get; }
	}

	public class ImportIEnumerableService : IImportIEnumerableService
	{
		public ImportIEnumerableService(IEnumerable<ISimpleObject> simpleObjects)
		{
			Count = simpleObjects.Count();
		}

		public int Count { get; private set; }
	}
}