using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IStaticPropertyClass
	{
		IBasicService BasicService { get; }
	}

	public class StaticPropertyClass : IStaticPropertyClass
	{
		public IBasicService BasicService { get; set; }

		public static IBasicService SomeOtherService { get; set; }
	}
}
