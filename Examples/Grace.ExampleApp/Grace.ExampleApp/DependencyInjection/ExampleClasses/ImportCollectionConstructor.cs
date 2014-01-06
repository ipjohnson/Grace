using System.Collections.Generic;
using System.Linq;

namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public interface IImportCollectionConstructor
	{
		int SimpleCount { get; }
	}

	public class ImportCollectionConstructor : IImportCollectionConstructor
	{
		public ImportCollectionConstructor(IEnumerable<ISimpleObject> simpleObjects)
		{
			SimpleCount = simpleObjects.Count();
		}

		public int SimpleCount { get; set; }
	}
}
