namespace Grace.UnitTests.Classes.Simple
{
	public class ImportMethodWithArgs
	{
		public IBasicService BasicService { get; private set; }

		public string StringParam { get; private set; }

		public int IntParam { get; private set; }

		public void ImportMethod(IBasicService basicService, string stringParam, int intParam)
		{
			BasicService = basicService;

			StringParam = stringParam;

			IntParam = intParam;
		}
	}
}