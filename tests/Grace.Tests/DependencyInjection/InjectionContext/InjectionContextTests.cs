using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.InjectionContext
{
    public class InjectionContextTests
    {
        [Fact]
        public void Container_InjectionContext_Convert_Type()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().ImportProperty(i => i.Count));

            var instance = container.Locate<IBasicService>(new { Count = 5.0 });

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Count);
        }

        [Fact]
        public void Container_InjectionContext_Convert_Mismatch_Type_Throws()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().ImportProperty(i => i.Count));

            IBasicService instance = null;

            Assert.Throws<LocateException>(() => instance = container.Locate<IBasicService>(new { Count = "Hello" }));
        }

        [Fact]
        public void InjectionContext_Keys_Empty()
        {
            var context = new Grace.DependencyInjection.Impl.InjectionContext(null);

            Assert.Equal(0, context.Keys.Count());
        }

        [Fact]
        public void InjectionContext_Keys()
        {
            var context = new Grace.DependencyInjection.Impl.InjectionContext(null);

            context.SetExtraData("Hello", "World");

            var keys = context.Keys.ToArray();

            Assert.Equal(1, keys.Length);
            Assert.Equal("Hello", keys[0]);
        }

        [Fact]
        public void InjectionContext_Value()
        {
            var context = new Grace.DependencyInjection.Impl.InjectionContext(null);

            context.SetExtraData("Hello", "World");

            var keys = context.Values.ToArray();

            Assert.Equal(1, keys.Length);
            Assert.Equal("World", keys[0]);
        }

        [Fact]
        public void InjectionContext_Value_Empty()
        {
            var context = new Grace.DependencyInjection.Impl.InjectionContext(null);

            Assert.Equal(0, context.Values.Count());
        }

        [Fact]
        public void InjectionContext_KVP()
        {
            var context = new Grace.DependencyInjection.Impl.InjectionContext(null);

            context.SetExtraData("Hello", "World");

            var keys = context.KeyValuePairs.ToArray();

            Assert.Equal(1, keys.Length);
            Assert.Equal("Hello", keys[0].Key);
            Assert.Equal("World", keys[0].Value);
        }


        [Fact]
        public void InjectionContext_KVP_Empty()
        {
            var context = new Grace.DependencyInjection.Impl.InjectionContext(null);

            Assert.Equal(0, context.KeyValuePairs.Count());
        }

        [Fact]
        public void InjectionContext_SetValue()
        {
            var context = new Grace.DependencyInjection.Impl.InjectionContext(null);

            context.SetExtraData(5, "Hello");

            Assert.Equal("Hello", context.GetExtraData(5));
        }


        [Fact]
        public void InjectionContext_SetValue_Override()
        {
            var context = new Grace.DependencyInjection.Impl.InjectionContext(null);

            context.SetExtraData(5, "Hello");

            Assert.Equal("Hello", context.GetExtraData(5));

            context.SetExtraData(5, "World");

            Assert.Equal("World", context.GetExtraData(5));
        }

        [Fact]
        public void InjectionContext_SetValue_Dont_Override()
        {
            var context = new Grace.DependencyInjection.Impl.InjectionContext(null);

            context.SetExtraData(5, "Hello");

            Assert.Equal("Hello", context.GetExtraData(5));

            context.SetExtraData(5, "World", replaceIfExists: false);

            Assert.Equal("Hello", context.GetExtraData(5));
        }

        [Fact]
        public void InjectionContext_GetValueByType()
        {
            var context = new Grace.DependencyInjection.Impl.InjectionContext(null);

            context.SetExtraData(5, "Hello");

            Assert.Equal("Hello", context.GetValueByType(typeof(string)));
        }
    }
}
