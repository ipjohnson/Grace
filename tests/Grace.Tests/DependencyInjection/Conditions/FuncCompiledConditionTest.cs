using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Conditions
{
    public class FuncCompiledConditionTest
    {
        [Fact]
        public void FuncCompiledCondition_Non_Runtime_Values()
        {
            var condition = new FuncCompiledCondition((strategy, context) => context.ActivationType == typeof(IBasicService));

            Assert.False(condition.IsRequestTimeCondition);
            Assert.False(condition.RequiresInjectionContext);
        }

        [Fact]
        public void FuncCompiledCondition_Calls_Func()
        {
            var condition = new FuncCompiledCondition((strategy, context) => context.ActivationType == typeof(IBasicService));

            Assert.True(condition.MeetsCondition(null,new StaticInjectionContext(typeof(IBasicService)), null));
        }

        [Fact]
        public void FuncCompiledCondition_Throw_Exception_When_Null_Func()
        {
            Assert.Throws<ArgumentNullException>(() => new FuncCompiledCondition(null));
        }
    }
}
