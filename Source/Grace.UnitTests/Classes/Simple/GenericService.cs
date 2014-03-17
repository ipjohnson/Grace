namespace Grace.UnitTests.Classes.Simple
{
	public interface IGenericService<T>
	{
	}

	public class GenericService<T> : IGenericService<T>
	{
	}

	public class ConstrainedService<T> : IGenericService<T> where T : IBasicService
	{

	}
}