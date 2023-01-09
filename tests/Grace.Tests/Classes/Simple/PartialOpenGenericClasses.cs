namespace Grace.Tests.Classes.Simple
{
	public interface IPartialInterface<T, T2>
	{
		
	}

	public class PartialOpenGenericString<T> : IPartialInterface<T,string>
	{

	}

}
