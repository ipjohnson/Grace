using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public interface IImportPropertyMultiple
	{
		ReadOnlyCollection<ISimpleObject> SimpleObjects { get; }

		int Count { get; }
	}

	public class ImportPropertyMultiple : IImportPropertyMultiple
	{
		public ReadOnlyCollection<ISimpleObject> SimpleObjects { get; set; }

		public int Count { get { return SimpleObjects.Count; } }
	}
}
