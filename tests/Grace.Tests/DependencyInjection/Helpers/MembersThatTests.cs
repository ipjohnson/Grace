using System;
using System.Reflection;
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
            Func<MemberInfo, bool> filter = MembersThat.Match(m => m.Name == nameof(MemberClass.Count));

            var countMember = typeof(MemberClass).GetProperty(nameof(MemberClass.Count));
            var customStringMember = typeof(MemberClass).GetProperty(nameof(MemberClass.CustomString));

            Assert.True(filter(countMember));
            Assert.False(filter(customStringMember));
        }
    }
}
