namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public interface IBasicService
	{
		int SomeMethod();
	}

	public class BasicService : IBasicService
	{
		public int SomeMethod()
		{
			return 0;
		}
	}
}
