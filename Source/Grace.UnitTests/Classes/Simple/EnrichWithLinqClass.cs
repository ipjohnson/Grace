using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IIntPropClass
	{
		int IntProp { get; set; }
	}

	public class EnrichWithLinqClass : IIntPropClass
	{
		public int IntProp { get; set; }
	}
}
