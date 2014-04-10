using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IIntConstructorClass
	{
		int TestValue { get; }
	}

	public class IntConstructorClass : IIntConstructorClass
	{
		public IntConstructorClass(int testValue)
		{
			TestValue = testValue;
		}

		public int TestValue { get; private set; }
	}
}
