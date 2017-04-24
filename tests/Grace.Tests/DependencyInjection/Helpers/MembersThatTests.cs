using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Grace.DependencyInjection;
using Xunit;

namespace Grace.Tests.DependencyInjection.Helpers
{
    public class MembersThatTests
    {
        public class MemberClass
        {
            public int Count { get; set; }

            public string CustomString { get; set; }
        }

        [Fact]
        public void MembersThat_Match()
        {
            Func<MemberInfo, bool> filter = MembersThat.Match(m => m.Name == "Count");

            var countMember = typeof(MemberClass).GetProperty("Count");
            var customStringMember = typeof(MemberClass).GetProperty("CustomString");

            Assert.True(filter(countMember));
            Assert.False(filter(customStringMember));
        }
    }
}
