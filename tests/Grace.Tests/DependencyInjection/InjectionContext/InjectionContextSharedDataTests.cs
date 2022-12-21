using System.Linq;
using Grace.DependencyInjection.Impl;
using Xunit;

namespace Grace.Tests.DependencyInjection.InjectionContext
{
    public class InjectionContextSharedDataTests
    {
        [Fact]
        public void InjectionContextSharedData_Keys()
        {
            var data = new InjectionContextSharedData();

            data.SetExtraData("Key",new object());

            var keys = data.Keys.ToArray();

            Assert.Single(keys);
            Assert.Equal("Key", keys[0]);
        }

        [Fact]
        public void InjectionContextSharedData_Values()
        {
            var data = new InjectionContextSharedData();

            data.SetExtraData("Key", "Value");

            var values = data.Values.ToArray();

            Assert.Single(values);
            Assert.Equal("Value", values[0]);
        }

        [Fact]
        public void InjectionContextSharedData_KVP()
        {
            var data = new InjectionContextSharedData();

            data.SetExtraData("Key", "Value");

            var pairs = data.KeyValuePairs.ToArray();

            Assert.Single(pairs);
            Assert.Equal("Key", pairs[0].Key);
            Assert.Equal("Value", pairs[0].Value);
        }
    }
}
