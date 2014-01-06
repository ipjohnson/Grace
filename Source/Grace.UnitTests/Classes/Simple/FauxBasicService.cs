namespace Grace.UnitTests.Classes.Simple
{
	public class FauxBasicService : IBasicService
	{
		public int Count { get; set; }

		public int TestMethod()
		{
			return 0;
		}
	}
}