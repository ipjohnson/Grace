using System.Collections;
using System.Collections.Generic;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.FauxClasses;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class BlackListTests
	{
		[Fact]
		public void BasicClassTest()
		{
			BlackList blackList = new BlackList { "Hello", "World" };

			List<string> newList = new List<string>(blackList);

			Assert.Equal(2, newList.Count);
			Assert.Equal("Hello", newList[0]);
			Assert.Equal("World", newList[1]);
		}

		[Fact]
		public void EnumerableTest()
		{
			BlackList blackList = new BlackList { "Hello", "World" };

			List<string> newList = new List<string>();

			foreach (object o in (IEnumerable)blackList)
			{
				newList.Add((string)o);
			}

			Assert.Equal(2, newList.Count);
			Assert.Equal("Hello", newList[0]);
			Assert.Equal("World", newList[1]);
		}

		[Fact]
		public void IsBlackedListedTest()
		{
			BlackList blackList = new BlackList { "Hello", "World" };

			Assert.True(blackList.IsExportStrategyBlackedOut(new FauxExportStrategy(() => new object())
			                                                 {
				                                                 ActivationName = "Hello",
			                                                 }));

			Assert.True(blackList.IsExportStrategyBlackedOut(new FauxExportStrategy(() => new object())
			                                                 {
				                                                 ActivationName = "World"
			                                                 }));

			Assert.False(blackList.IsExportStrategyBlackedOut(new FauxExportStrategy(() => new object())
			                                                  {
				                                                  ActivationName = "GoodBye"
			                                                  }));
		}
	}
}