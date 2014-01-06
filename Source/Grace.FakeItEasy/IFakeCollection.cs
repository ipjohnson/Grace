using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.FakeItEasy
{
	public interface IFakeCollection : IEnumerable<object>
	{
		void AddFake<T>(T newT, Action<T> assertAction);

		void Clear();

		void Assert();
	}
}
