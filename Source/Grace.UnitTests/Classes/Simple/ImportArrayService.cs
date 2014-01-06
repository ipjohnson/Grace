using Xunit;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IImportArrayService
	{
	}

	public class ImportArrayService : IImportArrayService
	{
		public ImportArrayService(ISimpleObject[] simpleObjects)
		{
			Assert.NotNull(simpleObjects);
			Assert.Equal(5, simpleObjects.Length);
		}
	}
}