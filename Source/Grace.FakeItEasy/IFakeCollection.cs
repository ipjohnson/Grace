using System;
using System.Collections.Generic;

namespace Grace.FakeItEasy
{
	public interface IFakeCollection : IEnumerable<object>
	{
		void AddFake<T>(T newT, Action<T> assertAction);

		void Clear();

		void Assert();
	}
}