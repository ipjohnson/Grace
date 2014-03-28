using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IOpenGenericPartiallyClosedInterface<T,T2,T3,T4>
	{
		
	}

	public class OpenGenericPartiallyClosedInterface<T,T2,T3,T4> : IOpenGenericPartiallyClosedInterface<T,T2,T3,T4>
	{

	}

	public class PartiallyClosedInterface<T, T2, T3> : IOpenGenericPartiallyClosedInterface<T, T2, T3, string>
	{
		
	}

	public class EvenMoreClosedInterface<T, T2> : IOpenGenericPartiallyClosedInterface<T, T2, double, string>
	{
		
	}
}
