using System;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Conditions
{
	public class WhenInjectedIntoConditionTests
	{
		[Fact]
		public void ConditionMeetTest()
		{
			WhenInjectedInto condition = new WhenInjectedInto(typeof(ImportPropertyService));

			bool conditionMeet =
				condition.ConditionMeet(new FauxInjectionScope(),
					new FauxInjectionContext
					{
						TargetInfo =
							new InjectionTargetInfo(typeof(ImportPropertyService),
							new Attribute[0],
							typeof(ImportPropertyService).GetProperty("BasicService"),
							new Attribute[0],
							new Attribute[0],
							null,
							null)
					},
					new FauxExportStrategy(() => new object()));

			Assert.True(conditionMeet);
		}

		[Fact]
		public void ConditionNotMeetTest()
		{
			WhenInjectedInto condition = new WhenInjectedInto(typeof(ImportPropertyService));

			bool conditionMeet =
				condition.ConditionMeet(new FauxInjectionScope(),
					new FauxInjectionContext
					{
						TargetInfo =
							new FauxInjectionTargetInfo { InjectionType = typeof(ImportConstructorService) }
					},
					new FauxExportStrategy(() => new object()));

			Assert.False(conditionMeet);
		}

		[Fact]
		public void ConditionTargetInfoNull()
		{
			WhenInjectedInto condition = new WhenInjectedInto(typeof(ImportPropertyService));

			bool conditionMeet =
				condition.ConditionMeet(new FauxInjectionScope(),
					new FauxInjectionContext(),
					new FauxExportStrategy(() => new object()));

			Assert.False(conditionMeet);
		}
	}
}