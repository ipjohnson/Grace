namespace Grace.UnitTests.Classes.Simple
{
	public class BasicServiceWrapper : IBasicService
	{
		public BasicServiceWrapper(IBasicService basicService)
		{
			BasicService = basicService;
		}

		public IBasicService BasicService { get; set; }

		public int Count { get; set; }

		public int TestMethod()
		{
			return 0;
		}
	}
}