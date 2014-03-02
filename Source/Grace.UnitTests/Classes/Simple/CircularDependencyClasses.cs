using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IDependencyA
	{
		
	}

	public interface IDependencyB
	{
		
	}

	public class DependencyA : IDependencyA
	{
		public DependencyA(IDependencyB dependencyB)
		{
			
		}
	}

	public class DependencyB : IDependencyB
	{
		public DependencyB(IDependencyA dependencyA)
		{
			
		}
	}
}
