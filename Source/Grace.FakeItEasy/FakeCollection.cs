using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::FakeItEasy;

namespace Grace.FakeItEasy
{
	public class FakeCollection : IFakeCollection
	{
		private List<Tuple<object, Action>> fakeCollection = new List<Tuple<object, Action>>();

		public void AddFake<T>(T newT, Action<T> assertAction)
		{
			Action assert;

			if (assertAction != null)
			{
				assert = () => assertAction(newT);
			}
			else
			{
				assert = () => { };
			}

			fakeCollection.Add(new Tuple<object, Action>(newT, assert));
		}

		public void Clear()
		{
			fakeCollection.Clear();
		}

		public void Assert()
		{
			foreach (Tuple<object, Action> tuple in fakeCollection)
			{
				tuple.Item2();
			}
		}

		public IEnumerator<object> GetEnumerator()
		{
			foreach (Tuple<object, Action> tuple in fakeCollection)
			{
				yield return tuple.Item1;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
