using System.IO;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.MemberInjection
{
    public class PropertyInjectionTests
    {
        [Fact]
        public void PropertyInjection_ImportMembers()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<PropertyInjectionService>().As<IPropertyInjectionService>().ImportMembers();
            });

            var propertyInjectionService = container.Locate<IPropertyInjectionService>();

            Assert.NotNull(propertyInjectionService);
            Assert.NotNull(propertyInjectionService.BasicService);
        }

        [Fact]
        public void PropertyInjection_ImportProperty()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<PropertyInjectionService>().As<IPropertyInjectionService>().ImportProperty(s => s.BasicService);
            });

            var propertyInjectionService = container.Locate<IPropertyInjectionService>();

            Assert.NotNull(propertyInjectionService);
            Assert.NotNull(propertyInjectionService.BasicService);
        }


        public class MultiplePropertyInject
        {
            [SomeTest]
            public IBasicService BasicService { get; set; }

            [SomeTest(TestValue = 10)]
            public IMultipleService MultipleService { get; set; }
        }

        [Fact]
        public void PropertyInjection_ImportMember_With_Members_Named_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultiplePropertyInject>().ImportMembers(MembersThat.AreNamed("BasicService"));
            });

            var instance = container.Locate<MultiplePropertyInject>();

            Assert.NotNull(instance);
            
            Assert.NotNull(instance.BasicService);
            
            Assert.Null(instance.MultipleService);
        }


        [Fact]
        public void PropertyInjection_ImportMember_With_Members_StartWith_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultiplePropertyInject>().ImportMembers(MembersThat.StartWith("Basic"));
            });

            var instance = container.Locate<MultiplePropertyInject>();

            Assert.NotNull(instance);

            Assert.NotNull(instance.BasicService);

            Assert.Null(instance.MultipleService);
        }

        [Fact]
        public void PropertyInjection_ImportMember_With_Members_EndsWith_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultiplePropertyInject>().ImportMembers(MembersThat.EndsWith("cService"));
            });

            var instance = container.Locate<MultiplePropertyInject>();

            Assert.NotNull(instance);

            Assert.NotNull(instance.BasicService);

            Assert.Null(instance.MultipleService);
        }

        [Fact]
        public void PropertyInjection_ImportMember_With_Members_Property_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultiplePropertyInject>().ImportMembers(MembersThat.AreProperty());
            });

            var instance = container.Locate<MultiplePropertyInject>();

            Assert.NotNull(instance);

            Assert.NotNull(instance.BasicService);

            Assert.NotNull(instance.MultipleService);
        }


        [Fact]
        public void PropertyInjection_ImportMember_With_Members_HaveAttribute_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultiplePropertyInject>().ImportMembers(MembersThat.HaveAttribute<SomeTestAttribute>());
            });

            var instance = container.Locate<MultiplePropertyInject>();

            Assert.NotNull(instance);

            Assert.NotNull(instance.BasicService);

            Assert.NotNull(instance.MultipleService);
        }

        [Fact]
        public void PropertyInjection_ImportMember_With_Members_HaveAttribute_Filter_With_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultiplePropertyInject>().ImportMembers(MembersThat.HaveAttribute<SomeTestAttribute>(attribute => attribute.TestValue == 10));
            });

            var instance = container.Locate<MultiplePropertyInject>();

            Assert.NotNull(instance);

            Assert.Null(instance.BasicService);

            Assert.NotNull(instance.MultipleService);
        }
    }
}
