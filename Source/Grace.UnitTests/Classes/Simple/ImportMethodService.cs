namespace Grace.UnitTests.Classes.Simple
{
	public interface IImportMethodService
	{
		IBasicService BasicService { get; }
	}

	public class ImportMethodService : IImportMethodService
	{
		public IBasicService BasicService { get; private set; }

		public void ImportMethod(IBasicService basicService)
		{
			BasicService = basicService;
		}
	}
}