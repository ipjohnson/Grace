using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
