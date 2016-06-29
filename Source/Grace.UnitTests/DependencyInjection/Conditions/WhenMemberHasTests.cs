using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Conditions
{
    public class WhenMemberHasTests
    {
        [Fact]
        public void ConditionMeetTest()
        {
            WhenMemberHas whenMemberHas = new WhenMemberHas(typeof(SomeTestAttribute));

            InjectionTargetInfo targetInfo =
                new InjectionTargetInfo(typeof(ImportPropertyService),
                    new Attribute[0],
                    typeof(ImportPropertyService).GetProperty("BasicService"),
                    ExportStrategyDependencyType.Property,
                    new Attribute[0],
                    new Attribute[] { new SomeTestAttribute() },
                    null,
                    null,
                    true,
                    null);

            bool conditionMeet =
                whenMemberHas.ConditionMeet(new FauxInjectionScope(),
                    new InjectionContext(null, new FauxInjectionScope()) { TargetInfo = targetInfo },
                    new FauxExportStrategy(() => new ImportPropertyService()));

            Assert.True(conditionMeet);
        }

        [Fact]
        public void ConditionNotMeetTest()
        {
            WhenClassHas whenMemberHas = new WhenClassHas(typeof(SomeTestAttribute));

            InjectionTargetInfo targetInfo =
                new InjectionTargetInfo(typeof(ImportPropertyService),
                                                new Attribute[0],
                                                typeof(ImportPropertyService).GetProperty("BasicService"),
                                                ExportStrategyDependencyType.Property,
                                                new Attribute[0],
                                                new Attribute[0],
                                                null,
                                                null,
                                                true,
                                                null);

            bool conditionMeet =
                whenMemberHas.ConditionMeet(new FauxInjectionScope(),
                    new InjectionContext(null, new FauxInjectionScope()),
                    new FauxExportStrategy(() => new ImportPropertyService()));

            Assert.False(conditionMeet);

            conditionMeet =
                whenMemberHas.ConditionMeet(new FauxInjectionScope(),
                    new InjectionContext(null, new FauxInjectionScope()) { TargetInfo = targetInfo },
                    new FauxExportStrategy(() => new ImportPropertyService()));

            Assert.False(conditionMeet);
        }
    }
}