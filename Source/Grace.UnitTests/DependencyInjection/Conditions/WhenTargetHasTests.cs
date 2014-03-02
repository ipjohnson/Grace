using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Conditions
{
	public class WhenTargetHasTests
	{
		[Fact]
		public void ConditionMeetTest()
		{
			WhenTargetHas whenTargetHas = new WhenTargetHas(typeof(SomeTestAttribute));

			InjectionTargetInfo targetInfo =
				new InjectionTargetInfo(typeof(ImportPropertyService),
					new Attribute[0],
					typeof(ImportPropertyService).GetProperty("BasicService"),
					new Attribute[] { new SomeTestAttribute() },
					new Attribute[0], 
					null,
					null);

			bool conditionMeet =
				whenTargetHas.ConditionMeet(new FauxInjectionScope(),
					new InjectionContext(null, new FauxInjectionScope()) { TargetInfo = targetInfo },
					new FauxExportStrategy(() => new ImportPropertyService()));

			Assert.True(conditionMeet);
		}

		[Fact]
		public void ConditionNotMeetTest()
		{
			WhenTargetHas whenTargetHas = new WhenTargetHas(typeof(SomeTestAttribute));

			InjectionTargetInfo targetInfo =
				new InjectionTargetInfo(typeof(ImportPropertyService),
					new Attribute[0],
					typeof(ImportPropertyService).GetProperty("BasicService"),
					new Attribute[0],
					new Attribute[0],
					null,
					null);

			bool conditionMeet =
				whenTargetHas.ConditionMeet(new FauxInjectionScope(),
					new InjectionContext(null, new FauxInjectionScope()),
					new FauxExportStrategy(() => new ImportPropertyService()));

			Assert.False(conditionMeet);

			conditionMeet =
				whenTargetHas.ConditionMeet(new FauxInjectionScope(),
					new InjectionContext(null, new FauxInjectionScope()) { TargetInfo = targetInfo },
					new FauxExportStrategy(() => new ImportPropertyService()));

			Assert.False(conditionMeet);
		}
	}
}