using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public interface IOpenGenericService<T, T2, T3>
	{
		void DoSomething();
	}

	public class OpenGenericService<T, T2, T3> : IOpenGenericService<T, T2, T3>
	{
		public void DoSomething()
		{
			
		}
	}

	public class PartiallyClosedGenericService<T, T2> : IOpenGenericService<T, T2, string>
	{
		public void DoSomething()
		{
			
		}
	}

	public class EvenMoreClosedGenericService<T> : IOpenGenericService<T, double, string>
	{
		public void DoSomething()
		{
			
		}
	}
}
