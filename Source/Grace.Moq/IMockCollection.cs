using System;
using System.Collections.Generic;
using Moq;

namespace Grace.Moq
{
	/// <summary>
	/// A collection of mock objects created by the container. You can locate this interface from the container
	/// </summary>
	public interface IMockCollection : IEnumerable<Mock>
	{
		/// <summary>
		/// Add a mock to the collection
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="newMock"></param>
		/// <param name="verifyMethod"></param>
		void AddMock<T>(Mock<T> newMock, Action<Mock<T>> verifyMethod) where T : class;

		/// <summary>
		/// Clear the collection of mock objects
		/// </summary>
		void Clear();

		/// <summary>
		/// Assert all mock objects created pass verification. 
		/// </summary>
		void Assert();
	}
}