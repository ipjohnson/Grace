using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grace.DependencyInjection.Impl.Expressions;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.DelegateRequest
{
    public class ActivationExpressionRequestTests
    {
        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void ActivationExpressionRequest_UniqueId(ActivationExpressionRequest request)
        {
            var uniqueId = request.UniqueId;

            Assert.Equal(uniqueId, request.UniqueId);
        }

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void ActivationExpressionRequest_PopWrapperPathNode_Return_Null_When_Empty(ActivationExpressionRequest request)
        {
            Assert.Null(request.PopWrapperPathNode());
        }

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void ActivationExpressionRequest_PopDecoratorPathNode_Return_Null_When_Empty(ActivationExpressionRequest request)
        {
            Assert.Null(request.PopDecoratorPathNode());
        }
        
        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void ActivationExpressionRequest_GetSetData(ActivationExpressionRequest request)
        {
            request.SetExtraData("Hello", 5);

            Assert.Equal(5, request.GetExtraData("Hello"));
        }
        
        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void ActivationExpressionRequest_Keys(ActivationExpressionRequest request)
        {
            request.SetExtraData("Hello", 5);
            
            var keys = request.Keys.ToArray();

            Assert.Equal(1, keys.Length);
            Assert.Equal("Hello", keys[0]);
        }

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void ActivationExpressionRequest_Value(ActivationExpressionRequest request)
        {
            request.SetExtraData("Hello", 5);

            var values = request.Values.ToArray();

            Assert.Equal(1, values.Length);
            Assert.Equal(5, values[0]);
        }
        
        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void ActivationExpressionRequest_KVP(ActivationExpressionRequest request)
        {
            request.SetExtraData("Hello", 5);

            var values = request.KeyValuePairs.ToArray();

            Assert.Equal(1, values.Length);
            Assert.Equal("Hello", values[0].Key);
            Assert.Equal(5, values[0].Value);
        }

        [Fact]
        public void PerDelegateData_Keys()
        {
            var data = new PerDelegateData();

            data.SetExtraData("Hello", 5);

            var keys = data.Keys.ToArray();

            Assert.Equal(1, keys.Length);
            Assert.Equal("Hello", keys[0]);
        }


        [Fact]
        public void PerDelegateData_Values()
        {
            var data = new PerDelegateData();

            data.SetExtraData("Hello", 5);

            var values = data.Values.ToArray();

            Assert.Equal(1, values.Length);
            Assert.Equal(5, values[0]);
        }
        
        [Fact]
        public void PerDelegateData_KVP()
        {
            var data = new PerDelegateData();

            data.SetExtraData("Hello", 5);

            var values = data.KeyValuePairs.ToArray();

            Assert.Equal(1, values.Length);
            Assert.Equal("Hello", values[0].Key);
            Assert.Equal(5, values[0].Value);
        }
    }
}
