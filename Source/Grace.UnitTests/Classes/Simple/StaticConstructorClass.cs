using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IStaticConstructorClass
	{
		
	}

	public class StaticConstructorClass : IStaticConstructorClass
	{
		private static readonly int test;

		static StaticConstructorClass()
		{
			test = 1;
		}
	}
}
