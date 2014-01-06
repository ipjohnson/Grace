namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public class MixedParameterConstructor
	{
		public MixedParameterConstructor(IBasicService basicService, string stringParam, int intParam)
		{
			BasicService = basicService;

			StringParam = stringParam;

			IntParam = intParam;
		}

		public IBasicService BasicService { get; private set; }

		public string StringParam { get; private set; }

		public int IntParam { get; private set; }
	}
}
