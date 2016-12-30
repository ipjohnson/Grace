using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Expressions
{
    [SubFixtureInitialize]
    public class ActivationStrategyCompilerTests
    {
        [Theory]
        [AutoData]
        public void ActivationStrategyCompiler_CreateNewRequest_Null_Type_Throws(ActivationStrategyCompiler compiler, IInjectionScope scope)
        {
            Assert.Throws<ArgumentNullException>(() => compiler.CreateNewRequest(null, 10, scope));
        }

        [Theory]
        [AutoData]
        public void ActivationStrategyCompiler_CreateNewRequest_Null_Scope_Throws(ActivationStrategyCompiler compiler)
        {
            Assert.Throws<ArgumentNullException>(() => compiler.CreateNewRequest(typeof(IBasicService), 10, null));
        }

        [Theory]
        [AutoData]
        public void ActivationStrategyCompiler_CreateNewResult_Null_Request_Throws(ActivationStrategyCompiler compiler)
        {
            Assert.Throws<ArgumentNullException>(() => compiler.CreateNewResult(null));
        }
    }
}
