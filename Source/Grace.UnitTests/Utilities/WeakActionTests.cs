using System;
using Grace.Utilities;
using Xunit;

namespace Grace.UnitTests.Utilities
{
	public class WeakActionTests
	{
		private static bool called;

		[Fact]
		public void GarbageCollect()
		{
			WeakAction action = NewAction();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			Assert.False(action.IsAlive);
		}

		private WeakAction NewAction()
		{
			ActionClass newClass = new ActionClass();

			WeakAction action = new WeakAction(newClass.Call);

			Assert.False(newClass.Called);

			action.Invoke();

			Assert.True(newClass.Called);

			return action;
		}

		[Fact]
		public void StaticTest()
		{
			WeakAction action = new WeakAction(StaticCall);

			called = false;

			action.Invoke();

			Assert.True(called);
		}

		private static void StaticCall()
		{
			called = true;
		}

		private class ActionClass
		{
			public bool Called { get; set; }

			public void Call()
			{
				Called = true;
			}
		}
	}
}