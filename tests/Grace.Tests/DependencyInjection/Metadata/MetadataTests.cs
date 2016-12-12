using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Metadata
{
    public class MetadataTests
    {
        [Fact]
        public void Meta_Enumerator()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().WithMetadata("Hello", "World"));

            var meta = container.Locate<Meta<IBasicService>>();

            Assert.NotNull(meta);
            Assert.Equal(1, meta.Metadata.Count);
            Assert.Equal(1, meta.Metadata.Count());
            Assert.Equal(1, ((IEnumerable)meta.Metadata).OfType<KeyValuePair<object, object>>().Count());

            var data = meta.Metadata.First();

            Assert.Equal("Hello", data.Key);
            Assert.Equal("World", data.Value);
        }

        [Fact]
        public void Meta_Keys_And_Values()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().WithMetadata("Hello", "World"));

            var meta = container.Locate<Meta<IBasicService>>();

            Assert.NotNull(meta);

            Assert.Equal(1, meta.Metadata.Keys.Count());
            Assert.True(meta.Metadata.ContainsKey("Hello"));
            Assert.True(meta.Metadata.Keys.Contains("Hello"));

            Assert.Equal(1, meta.Metadata.Values.Count());
            Assert.True(meta.Metadata.Values.Contains("World"));
        }

        [Fact]
        public void Meta_TryGetValue()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().WithMetadata("Hello", "World"));

            var meta = container.Locate<Meta<IBasicService>>();

            Assert.NotNull(meta);

            object value;
            Assert.False(meta.Metadata.TryGetValue("GoodBye", out value));
            Assert.True(meta.Metadata.TryGetValue("Hello", out value));
            Assert.Equal("World", value);
        }

        [Fact]
        public void Meta_Matches()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().WithMetadata("Hello", "World"));

            var meta = container.Locate<Meta<IBasicService>>();

            Assert.NotNull(meta);
            Assert.False(meta.Metadata.MetadataMatches("Hello","Earth"));
            Assert.False(meta.Metadata.MetadataMatches("GoodBye", "Earth"));
            Assert.False(meta.Metadata.MetadataMatches("Hello", null));
            Assert.True(meta.Metadata.MetadataMatches("Hello", "World"));
        }
        
        [Fact]
        public void Meta_Matches_Null()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().WithMetadata("Hello", null));

            var meta = container.Locate<Meta<IBasicService>>();

            Assert.NotNull(meta);
            Assert.False(meta.Metadata.MetadataMatches("Hello", "Earth"));
            Assert.True(meta.Metadata.MetadataMatches("Hello", null));
        }
    }
}
