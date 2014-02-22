namespace Grace.UnitTests.Classes.Simple
{
	public interface ILazyService
	{
	}

	public class LazyService : ILazyService
	{
		public LazyService()
		{
			Created = true;
		}

		public static bool Created { get; set; }
	}
}