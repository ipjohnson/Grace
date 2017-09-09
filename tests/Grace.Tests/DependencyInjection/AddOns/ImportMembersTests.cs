using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Grace.Tests.Classes.Simple;
using Grace.Tests.DependencyInjection.MemberInjection;
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
        public void ImportMembers_Generic_Property()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ImportMembers<IBasicService>();
                c.ExportAs<BasicService, IBasicService>();
            });

            var instance = container.Locate<ImportPropertyClass>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
        }


        public class MultiplePropertyInject
        {
            [Import]
            public IBasicService BasicService { get; set; }

            [Import]
            public IMultipleService MultipleService { get; set; }
        }

        [Fact]
        public void ImportMembers_Filtered()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ImportMembers(MembersThat.HaveAttribute<ImportAttribute>());
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultiplePropertyInject>();
            });

            var instance = container.Locate<MultiplePropertyInject>();

            Assert.NotNull(instance);

            Assert.NotNull(instance.BasicService);

            Assert.NotNull(instance.MultipleService);
        }

        public class KeyedMultiplePropertyInject
        {
            [Import(Key = "A")]
            public IBasicService BasicServiceA { get; set; }


            [Import(Key = "B")]
            public IBasicService BasicServiceB { get; set; }
        }

        public class OtherBasicService : IBasicService
        {
            public int Count { get; set; }

            public int TestMethod()
            {
                return Count;
            }
        }

        [Fact]
        public void ImportMembers_Filtered_Keyed()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ImportMembers(MembersThat.HaveAttribute<ImportAttribute>());
                c.Export<BasicService>().AsKeyed<IBasicService>("A");
                c.Export<OtherBasicService>().AsKeyed<IBasicService>("B");
                c.Export<KeyedMultiplePropertyInject>();
            });

            var instance = container.Locate<KeyedMultiplePropertyInject>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicServiceA);
            Assert.IsType<BasicService>(instance.BasicServiceA);

            Assert.NotNull(instance.BasicServiceB);
            Assert.IsType<OtherBasicService>(instance.BasicServiceB);
        }
    }
}
