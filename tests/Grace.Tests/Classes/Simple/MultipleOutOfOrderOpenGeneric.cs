namespace Grace.UnitTests.Classes.Simple
{
	public interface IMultipleOutOfOrderOpenGeneric<T,T2,T3,T4>
	{
		
	}

	public class MultipleOutOfOrderOpenGeneric<T,T2,T3,T4> : IMultipleOutOfOrderOpenGeneric<T4,T3,T2,T> where T4 : class 
	{

	}
}
