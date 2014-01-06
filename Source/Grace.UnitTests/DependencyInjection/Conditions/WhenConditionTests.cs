using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Conditions
{
	public class WhenConditionTests
	{
		[Fact]
		public void ConditionMeetTest()
		{
			WhenCondition condition = new WhenCondition((x, y, z) => y.GetExtraData("A") != null);
			InjectionContext context = new InjectionContext(null, null);

			Assert.False(condition.ConditionMeet(null, context, null));

			context.SetExtraData("A", true);

			Assert.True(condition.ConditionMeet(null, context, null));
		}
	}
}