namespace Grace.UnitTests.Classes.Simple
{
	public interface IBasicService
	{
		int Count { get; set; }

		int TestMethod();
	}

	public class BasicService : IBasicService
	{
		public int Count { get; set; }

		public int TestMethod()
		{
			return 0;
		}
	}
}