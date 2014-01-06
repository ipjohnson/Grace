using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace Grace.Moq
{
	/// <summary>
	/// Collection of Mock object
	/// </summary>
	public class MockCollection : IMockCollection
	{
		private readonly List<Tuple<Mock, Action>> mocks = new List<Tuple<Mock, Action>>();

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<Mock> GetEnumerator()
		{
			return mocks.Select(x => x.Item1).GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Add a mock to the collection
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="newMock"></param>
		/// <param name="verifyMethod"></param>
		public void AddMock<T>(Mock<T> newMock, Action<Mock<T>> verifyMethod) where T : class
		{
			Action verifyAction;

			if (verifyMethod == null)
			{
				verifyAction = newMock.Verify;
			}
			else
			{
				verifyAction = () => verifyMethod(newMock);
			}

			mocks.Add(new Tuple<Mock, Action>(newMock, verifyAction));
		}

		/// <summary>
		/// Clear the collection of mock objects
		/// </summary>
		public void Clear()
		{
			mocks.Clear();
		}

		/// <summary>
		/// Verify all mock objects created pass verification. 
		/// </summary>
		public void Assert()
		{
			foreach (Tuple<Mock, Action> tuple in mocks)
			{
				tuple.Item2();
			}
		}
	}
}