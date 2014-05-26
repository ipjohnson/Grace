using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
	public class OtherBasicService : IBasicService
	{
		public int Count { get; set; }

		public int TestMethod()
		{
			return Count;
		}
	}
}
