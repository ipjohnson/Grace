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
					ExportEnvironment.RunTime,
					DependencyInjectionContainer.CompareExportStrategies);

			collection.AddExport(new FauxExportStrategy(() => 5));

			Assert.Equal(5, collection.Activate(null, null, new FauxInjectionContext(), null, null));
		}

		[Fact]
		public void DisposeTwiceTest()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					ExportEnvironment.RunTime,
					DependencyInjectionContainer.CompareExportStrategies);

			collection.AddExport(new FauxExportStrategy(() => 5));

			collection.Dispose();

			collection.Dispose();
		}

		[Fact]
		public void RemoveTest()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					ExportEnvironment.RunTime,
					DependencyInjectionContainer.CompareExportStrategies);

			FauxExportStrategy tenExport = new FauxExportStrategy(() => 10) { Priority = 10 };

			collection.AddExport(new FauxExportStrategy(() => 5) { Priority = 5 });
			collection.AddExport(tenExport);
			collection.AddExport(new FauxExportStrategy(() => 1) { Priority = 1 });

			Assert.Equal(10, collection.Activate(null, null, new FauxInjectionContext(), null, null));

			collection.RemoveExport(tenExport);

			Assert.Equal(5, collection.Activate(null, null, new FauxInjectionContext(), null, null));
			Assert.Equal(2, collection.ActivateAll<int>(new FauxInjectionContext(), null, null).Count());
		}

		[Fact]
		public void PrioritySortTest()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					ExportEnvironment.RunTime,
					DependencyInjectionContainer.CompareExportStrategies);

			collection.AddExport(new FauxExportStrategy(() => 5) { Priority = 5 });
			collection.AddExport(new FauxExportStrategy(() => 10) { Priority = 10 });
			collection.AddExport(new FauxExportStrategy(() => 1) { Priority = 1 });

			Assert.Equal(10, collection.Activate(null, null, new FauxInjectionContext(), null, null));
		}

		[Fact]
		public void RunTimeSortTest()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					ExportEnvironment.RunTime,
					DependencyInjectionContainer.CompareExportStrategies);

			collection.AddExport(new FauxExportStrategy(() => ExportEnvironment.RunTime)
			                     {
				                     Environment = ExportEnvironment.RunTime
			                     });
			collection.AddExport(new FauxExportStrategy(() => ExportEnvironment.DesignTime)
			                     {
				                     Environment =
					                     ExportEnvironment.DesignTime
			                     });
			collection.AddExport(new FauxExportStrategy(() => ExportEnvironment.UnitTest)
			                     {
				                     Environment =
					                     ExportEnvironment.UnitTest
			                     });

			Assert.Equal(ExportEnvironment.RunTime, collection.Activate(null, null, new FauxInjectionContext(), null, null));

			Assert.Equal(3, collection.ActivateAll<ExportEnvironment>(new FauxInjectionContext(), null, null).Count());
		}

		[Fact]
		public void DesignTimeSortTest()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					ExportEnvironment.DesignTime,
					DependencyInjectionContainer.CompareExportStrategies);

			collection.AddExport(new FauxExportStrategy(() => ExportEnvironment.RunTime)
			                     {
				                     Environment = ExportEnvironment.RunTime
			                     });
			collection.AddExport(new FauxExportStrategy(() => ExportEnvironment.DesignTime)
			                     {
				                     Environment =
					                     ExportEnvironment.DesignTime
			                     });
			collection.AddExport(new FauxExportStrategy(() => ExportEnvironment.UnitTest)
			                     {
				                     Environment =
					                     ExportEnvironment.UnitTest
			                     });

			Assert.Equal(ExportEnvironment.DesignTime, collection.Activate(null, null, new FauxInjectionContext(), null, null));

			Assert.Equal(3, collection.ActivateAll<ExportEnvironment>(new FauxInjectionContext(), null, null).Count());
		}

		[Fact]
		public void UnitTestSortTest()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					ExportEnvironment.UnitTest,
					DependencyInjectionContainer.CompareExportStrategies);

			collection.AddExport(new FauxExportStrategy(() => ExportEnvironment.RunTime)
			                     {
				                     Environment = ExportEnvironment.RunTime
			                     });
			collection.AddExport(new FauxExportStrategy(() => ExportEnvironment.DesignTime)
			                     {
				                     Environment =
					                     ExportEnvironment.DesignTime
			                     });
			collection.AddExport(new FauxExportStrategy(() => ExportEnvironment.UnitTest)
			                     {
				                     Environment =
					                     ExportEnvironment.UnitTest
			                     });

			Assert.Equal(ExportEnvironment.UnitTest, collection.Activate(null, null, new FauxInjectionContext(), null, null));

			Assert.Equal(3, collection.ActivateAll<ExportEnvironment>(new FauxInjectionContext(), null, null).Count());
		}

		[Fact]
		public void ActivateAll()
		{
			ExportStrategyCollection collection =
				new ExportStrategyCollection(new FauxInjectionScope(),
					ExportEnvironment.RunTime,
					DependencyInjectionContainer.CompareExportStrategies);

			collection.AddExport(new FauxExportStrategy(() => 5) { Priority = 5 });
			collection.AddExport(new FauxExportStrategy(() => 10) { Priority = 10 });
			collection.AddExport(new FauxExportStrategy(() => 1) { Priority = 1 });

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
					ExportEnvironment.RunTime,
					DependencyInjectionContainer.CompareExportStrategies);

			collection.AddExport(new FauxExportStrategy(() => 5) { Priority = 5 });
			collection.AddExport(new FauxExportStrategy(() => 10) { Priority = 10 });
			collection.AddExport(new FauxExportStrategy(() => 1) { Priority = 1 });

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
					ExportEnvironment.RunTime,
					DependencyInjectionContainer.CompareExportStrategies);

			object activated = collection.Activate(null, null, new FauxInjectionContext(), null, null);

			Assert.Null(activated);
		}
	}
}