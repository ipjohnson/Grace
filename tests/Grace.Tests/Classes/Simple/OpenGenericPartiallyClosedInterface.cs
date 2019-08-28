using Grace.Tests.Classes.Simple;

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

    public interface IClassConstraint<THub, T> where THub : Hub<T> where T : class
    {

    }

    public class ClassConstraint<THub, T> : IClassConstraint<THub, T> where THub : Hub<T> where T : class
    {

    }

    public class Hub<T>
    {

    }

    public class InheritHub : Hub<BasicService>
    {

    }
}
