using System;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection.Attributes;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Attributes
{
	public class ExportAttributeTests
	{
		[Fact]
		public void DefaultAttributeTest()
		{
			ExportAttribute attribute = new ExportAttribute();

			IEnumerable<string> exportNames = attribute.ProvideExportNames(typeof(BasicService));

			Assert.NotNull(exportNames);
			Assert.Equal(0, exportNames.Count());
		}

		[Fact]
		public void NullArgTest()
		{
			try
			{
				ExportAttribute exportAttribute = new ExportAttribute((string)null);

				throw new Exception("ExportAttribute should throw exception on null string");
			}
			catch (ArgumentNullException argumentNull)
			{
				Assert.NotNull(argumentNull);
			}

			try
			{
				ExportAttribute exportAttribute = new ExportAttribute((Type)null);

				throw new Exception("ExportAttribute should throw exception on null string");
			}
			catch (ArgumentNullException argumentNull)
			{
				Assert.NotNull(argumentNull);
			}
		}

		[Fact]
		public void SingleNameAttributeTest()
		{
			const string testExport = "TestExport";
			ExportAttribute attribute = new ExportAttribute(testExport);

			IEnumerable<string> exportNames = attribute.ProvideExportNames(typeof(BasicService));

			Assert.NotNull(exportNames);
			Assert.Equal(1, exportNames.Count());
			Assert.Equal(testExport, exportNames.First());
		}

		[Fact]
		public void MultipleNameAttributeTest()
		{
			const string testExport = "TestExport";
			const string secondExport = "SecondExport";
			ExportAttribute attribute = new ExportAttribute(testExport, secondExport);

			IEnumerable<string> exportNames = attribute.ProvideExportNames(typeof(BasicService));

			Assert.NotNull(exportNames);
			Assert.Equal(2, exportNames.Count());
			Assert.Equal(testExport, exportNames.First());
			Assert.Equal(secondExport, exportNames.Last());
		}

		[Fact]
		public void SingleTypeAttributeTest()
		{
			ExportAttribute attribute = new ExportAttribute(typeof(BasicService));

			IEnumerable<Type> exportTypes = attribute.ProvideExportTypes(typeof(BasicService));

			Assert.NotNull(exportTypes);
			Assert.Equal(1, exportTypes.Count());
			Assert.Equal(typeof(BasicService), exportTypes.First());
		}

		[Fact]
		public void MultipleTypeAttributeTest()
		{
			ExportAttribute attribute = new ExportAttribute(typeof(BasicService), typeof(DisposableService));

			IEnumerable<Type> exportTypes = attribute.ProvideExportTypes(typeof(BasicService));

			Assert.NotNull(exportTypes);
			Assert.Equal(2, exportTypes.Count());
			Assert.Equal(typeof(BasicService), exportTypes.First());
			Assert.Equal(typeof(DisposableService), exportTypes.Last());
		}
	}
}