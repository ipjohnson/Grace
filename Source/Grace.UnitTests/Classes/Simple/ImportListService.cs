using System.Collections.Generic;

namespace Grace.UnitTests.Classes.Simple
{
	public class ImportListService
	{
		public ImportListService(List<ISimpleObject> simpleObjects)
		{
			SimpleObjects = simpleObjects;
		}

		public List<ISimpleObject> SimpleObjects { get; private set; }
	}
}