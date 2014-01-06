using Grace.DependencyInjection.Attributes;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Attributes
{
	public class ImportAttributeTests
	{
		[Fact]
		public void TypeImportTest()
		{
			ImportAttribute importAttribute = new ImportAttribute();

			ImportAttributeInfo info =
				importAttribute.ProvideImportInfo(typeof(IBasicService), "Property");

			Assert.NotNull(info);
			Assert.Null(info.ImportName);
			Assert.Null(info.ImportKey);
			Assert.True(info.IsRequired);
		}

		[Fact]
		public void NameImportTest()
		{
			string name = "TestName";
			ImportAttribute importAttribute = new ImportAttribute { Name = name };

			ImportAttributeInfo info =
				importAttribute.ProvideImportInfo(typeof(IBasicService), "Property");

			Assert.NotNull(info);
			Assert.Equal(name, info.ImportName);
			Assert.Null(info.ImportKey);
			Assert.True(info.IsRequired);
		}

		[Fact]
		public void RequiredTest()
		{
			ImportAttribute importAttribute = new ImportAttribute { Required = false };

			ImportAttributeInfo info =
				importAttribute.ProvideImportInfo(typeof(IBasicService), "Property");

			Assert.NotNull(info);
			Assert.Null(info.ImportName);
			Assert.Null(info.ImportKey);
			Assert.False(info.IsRequired);
		}

		[Fact]
		public void ImportKeyTest()
		{
			ImportAttribute importAttribute = new ImportAttribute { Key = 5 };

			ImportAttributeInfo info =
				importAttribute.ProvideImportInfo(typeof(IBasicService), "Property");

			Assert.NotNull(info);
			Assert.Null(info.ImportName);
			Assert.Equal(5, info.ImportKey);
			Assert.True(info.IsRequired);
		}
	}
}