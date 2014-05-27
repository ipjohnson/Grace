using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.UnitTests.Classes.Simple
{
	public class ImportKeyedLocateDelegate
	{
		private KeyedLocateDelegate<int, ISimpleObject> locateDelegate;

		public ImportKeyedLocateDelegate(KeyedLocateDelegate<int, ISimpleObject> locateDelegate)
		{
			this.locateDelegate = locateDelegate;
		}

		public ISimpleObject Locate(int key)
		{
			return locateDelegate(key);
		}
	}
}
