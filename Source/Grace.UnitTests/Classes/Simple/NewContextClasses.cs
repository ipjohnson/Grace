using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IContextSingleton
	{
		
	}

	public class ContextSingleton : IContextSingleton
	{
		
	}

	public interface IContextClassA
	{
		IContextSingleton ContextSingleton { get; }

		IContextClassB ContextClassB { get; }
	}

	
	public interface IContextClassB
	{
		IContextSingleton ContextSingleton { get; }

		IContextClassC ContextClassC { get; }
	}
	
	public interface IContextClassC
	{
		IContextSingleton ContextSingleton { get; }
	}

	public class ContextClassA : IContextClassA
	{
		public ContextClassA(IContextClassB contextClassB, IContextSingleton contextSingleton)
		{
			ContextSingleton = contextSingleton;
			ContextClassB = contextClassB;
		}

		public IContextSingleton ContextSingleton { get; private set; }

		public IContextClassB ContextClassB { get; private set; }
	}


	public class ContextClassB : IContextClassB
	{
		public ContextClassB(IContextClassC contextClassC, IContextSingleton contextSingleton)
		{
			ContextSingleton = contextSingleton;

			ContextClassC = contextClassC;
		}

		public IContextSingleton ContextSingleton { get; private set; }

		public IContextClassC ContextClassC { get; private set; }
	}
	
	public class ContextClassC : IContextClassC
	{
		public ContextClassC(IContextSingleton contextSingleton)
		{
			ContextSingleton = contextSingleton;
		}

		public IContextSingleton ContextSingleton { get; private set; }
	}
}
