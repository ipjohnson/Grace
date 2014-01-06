using Xunit;

namespace Grace.UnitTests.Classes.Simple
{
	public class ImportAllTypes
	{
		public ImportAllTypes(IBasicService basicService)
		{
			Assert.NotNull(basicService);
		}

		public IImportConstructorService ImportConstructorService { get; set; }

		public IImportPropertyService PropertyService { get; set; }

		public void ImportMethod(IImportConstructorService constructorService)
		{
			ImportConstructorService = constructorService;
		}
	}
}