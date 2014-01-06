using System;
using Grace.Utilities;
using Xunit;

namespace Grace.UnitTests.Utilities
{
	public class WeakFuncTests
	{
		private static bool Called { get; set; }

		[Fact]
		public void GarbageCollectionTest()
		{
			WeakFunc<int> weakFunc = CreateFunc<int>();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			Assert.False(weakFunc.IsAlive);

			weakFunc.Invoke();
		}

		private WeakFunc<T> CreateFunc<T>()
		{
			FuncClass<T> callClass = new FuncClass<T>();

			WeakFunc<T> returnValue = new WeakFunc<T>(callClass.Call);

			Assert.False(callClass.Called);

			returnValue.Invoke();

			Assert.True(callClass.Called);

			return returnValue;
		}

		[Fact]
		public void StaticMethodTest()
		{
			WeakFunc<int> weakFunc = new WeakFunc<int>(FuncMethod<int>);

			Called = false;

			weakFunc.Invoke();

			Assert.True(Called);
		}

		private static T FuncMethod<T>()
		{
			Called = true;

			return default(T);
		}

		private class FuncClass<T>
		{
			public bool Called { get; set; }

			public T Call()
			{
				Called = true;

				return default(T);
			}
		}
	}
}