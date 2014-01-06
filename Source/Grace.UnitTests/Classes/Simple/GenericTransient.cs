using Xunit;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IGenericTransient<T>
	{
	}

	public class GenericTransient<T> : IGenericTransient<T>
	{
		public GenericTransient(IGenericService<T> service)
		{
			Assert.NotNull(service);
		}
	}
}