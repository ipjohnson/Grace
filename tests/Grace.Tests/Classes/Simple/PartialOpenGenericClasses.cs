using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
	public interface IPartialInterface<T, T2>
	{
		
	}

	public class PartialOpenGenericString<T> : IPartialInterface<T,string>
	{

	}

}
