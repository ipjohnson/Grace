namespace Grace.UnitTests.Classes.Simple
{
	public class BaseGenericClass<T, T2, T3, T4>
	{
		
	}

	public class PartialClosedClass<T, T2, T3> : BaseGenericClass<T, T2, T3, double>
	{
		
	}

	public class EvenMoreClosedClass<T, T2> : PartialClosedClass<T, T2, string>
	{
		
	}
}
