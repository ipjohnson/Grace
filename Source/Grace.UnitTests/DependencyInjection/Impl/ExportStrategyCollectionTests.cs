using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.FauxClasses;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class ExportStrategyCollectionTests
	{
		[Fact]
		public void ActivateTest()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					null);

			collection.AddExport(new FauxExportStrategy(() => 5), null);

			Assert.Equal(5, collection.Activate(null, null, new FauxInjectionContext(), null, null));
		}

		[Fact]
		public void DisposeTwiceTest()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					null);

			collection.AddExport(new FauxExportStrategy(() => 5), null);

			collection.Dispose();

			collection.Dispose();
		}

		[Fact]
		public void RemoveTest()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					null);

			FauxExportStrategy tenExport = new FauxExportStrategy(() => 10) { Priority = 10 };

			collection.AddExport(new FauxExportStrategy(() => 5) { Priority = 5 }, null);
            collection.AddExport(tenExport, null);
            collection.AddExport(new FauxExportStrategy(() => 1) { Priority = 1 }, null);

			Assert.Equal(10, collection.Activate(null, null, new FauxInjectionContext(), null, null));

            collection.RemoveExport(tenExport, null);

			Assert.Equal(5, collection.Activate(null, null, new FauxInjectionContext(), null, null));
			Assert.Equal(2, collection.ActivateAll<int>(new FauxInjectionContext(), null, null).Count());
		}

		[Fact]
		public void PrioritySortTest()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					null);

            collection.AddExport(new FauxExportStrategy(() => 5) { Priority = 5 }, null);
            collection.AddExport(new FauxExportStrategy(() => 10) { Priority = 10 }, null);
            collection.AddExport(new FauxExportStrategy(() => 1) { Priority = 1 }, null);

			Assert.Equal(10, collection.Activate(null, null, new FauxInjectionContext(), null, null));
		}
        
		[Fact]
		public void ActivateAll()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					null);

            collection.AddExport(new FauxExportStrategy(() => 5) { Priority = 5 }, null);
            collection.AddExport(new FauxExportStrategy(() => 10) { Priority = 10 }, null);
            collection.AddExport(new FauxExportStrategy(() => 1) { Priority = 1 }, null);

			List<int> exports = new List<int>(collection.ActivateAll<int>(new FauxInjectionContext(), null, null));

			Assert.Equal(3, exports.Count);
			Assert.Equal(10, exports[0]);
			Assert.Equal(5, exports[1]);
			Assert.Equal(1, exports[2]);
		}

		[Fact]
		public void FilterTest()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					null);

            collection.AddExport(new FauxExportStrategy(() => 5) { Priority = 5 }, null);
            collection.AddExport(new FauxExportStrategy(() => 10) { Priority = 10 }, null);
            collection.AddExport(new FauxExportStrategy(() => 1) { Priority = 1 }, null);

			List<int> exports =
				new List<int>(collection.ActivateAll<int>(new FauxInjectionContext(),
					(context, strategy) => strategy.Priority % 2 == 1, null));

			Assert.Equal(2, exports.Count);
			Assert.Equal(5, exports[0]);
			Assert.Equal(1, exports[1]);
		}

		[Fact]
		public void EmptyActivate()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					null);

			object activated = collection.Activate(null, null, new FauxInjectionContext(), null, null);

			Assert.Null(activated);
		}
	}
}