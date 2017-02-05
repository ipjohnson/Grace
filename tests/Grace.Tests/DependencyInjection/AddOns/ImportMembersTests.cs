using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.AddOns
{
    public class ImportMembersTests
    {
        public class ImportPropertyClass
        {
            public IBasicService BasicService { get; set; }
        }

        [Fact]
        public void ImportMembers_Property()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ImportMember<IBasicService>();
                c.ExportAs<BasicService, IBasicService>();
            });

            var instance = container.Locate<ImportPropertyClass>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
        }
    }
}
