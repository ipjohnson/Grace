using System;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IMultipleOpenGeneric<T,T2,T3,T4>
	{
		
	}

	public class MultipleOpenGenericConstrained<T,T2,T3,T4> : IMultipleOpenGeneric<T,T2,T3,T4>
		where T : new()
		where T2 : class
		where T3 : IComparable, IConvertible, IEquatable<bool>
		where T4 : struct 
	{

	}
}
