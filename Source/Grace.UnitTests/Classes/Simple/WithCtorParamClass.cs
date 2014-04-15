namespace Grace.UnitTests.Classes.Simple
{
	public interface IWithCtorParamClass
	{
		IBasicService BasicService { get; }

		string StringParam { get; }

		int IntParam { get; }
	}

	public class WithCtorParamClass : IWithCtorParamClass
	{
		public WithCtorParamClass(string stringParam, int intParam, IBasicService basicService)
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