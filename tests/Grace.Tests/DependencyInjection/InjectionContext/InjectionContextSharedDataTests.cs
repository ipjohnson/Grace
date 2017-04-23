using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            Assert.Equal(1, keys.Length);
            Assert.Equal("Key", keys[0]);
        }

        [Fact]
        public void InjectionContextSharedData_Values()
        {
            var data = new InjectionContextSharedData();

            data.SetExtraData("Key", "Value");

            var values = data.Values.ToArray();

            Assert.Equal(1, values.Length);
            Assert.Equal("Value", values[0]);
        }

        [Fact]
        public void InjectionContextSharedData_KVP()
        {
            var data = new InjectionContextSharedData();

            data.SetExtraData("Key", "Value");

            var pairs = data.KeyValuePairs.ToArray();

            Assert.Equal(1, pairs.Length);
            Assert.Equal("Key", pairs[0].Key);
            Assert.Equal("Value", pairs[0].Value);
        }
    }
}
