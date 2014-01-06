namespace Grace.UnitTests.Classes.Simple
{
	public interface IImportPropertyService
	{
		IBasicService BasicService { get; }
	}

	[SomeTest]
	public class ImportPropertyService : IImportPropertyService
	{
		[SomeTest]
		public IBasicService BasicService { get; set; }
	}
}