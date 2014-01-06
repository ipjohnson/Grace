namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public interface IImportConstructor
	{
		IBasicService BasicService { get; }
	}

	public class ImportConstructor : IImportConstructor
	{
		public ImportConstructor(IBasicService basicService)
		{
			BasicService = basicService;
		}

		public IBasicService BasicService { get; private set; }

		public int SomeMethod()
		{
			return BasicService.SomeMethod();
		}
	}
}
