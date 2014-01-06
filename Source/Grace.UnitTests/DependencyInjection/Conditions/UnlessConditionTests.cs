using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Conditions
{
	public class UnlessConditionTests
	{
		[Fact]
		public void ConditionMeetTest()
		{
			UnlessCondition condition = new UnlessCondition((x, y, z) => y.GetExtraData("A") != null);
			InjectionContext context = new InjectionContext(null, null);

			Assert.True(condition.ConditionMeet(null, context, null));

			context.SetExtraData("A", true);

			Assert.False(condition.ConditionMeet(null, context, null));
		}
	}
}