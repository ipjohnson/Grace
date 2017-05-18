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
                
        [Fact]
        public void Meta_Enumerable()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>().WithMetadata("Key", 1);
                c.Export<MultipleService2>().As<IMultipleService>().WithMetadata("Key", 2);
                c.Export<MultipleService3>().As<IMultipleService>().WithMetadata("Key", 3);
                c.Export<MultipleService4>().As<IMultipleService>().WithMetadata("Key", 4);
                c.Export<MultipleService5>().As<IMultipleService>().WithMetadata("Key", 5);
            });

            var multipleServices = container.Locate<IEnumerable<Meta<IMultipleService>>>().ToArray();
            
            Assert.Equal(5, multipleServices.Length);

            Assert.Equal(1, multipleServices[0].Metadata["Key"]);
            Assert.IsType<MultipleService1>(multipleServices[0].Value);

            Assert.Equal(2, multipleServices[1].Metadata["Key"]);
            Assert.IsType<MultipleService2>(multipleServices[1].Value);

            Assert.Equal(3, multipleServices[2].Metadata["Key"]);
            Assert.IsType<MultipleService3>(multipleServices[2].Value);

            Assert.Equal(4, multipleServices[3].Metadata["Key"]);
            Assert.IsType<MultipleService4>(multipleServices[3].Value);

            Assert.Equal(5, multipleServices[4].Metadata["Key"]);
            Assert.IsType<MultipleService5>(multipleServices[4].Value);
        }

        [Fact]
        public void Meta_Enumerable_Dependent()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>().WithMetadata("Key", 1);
                c.Export<MultipleService2>().As<IMultipleService>().WithMetadata("Key", 2);
                c.Export<MultipleService3>().As<IMultipleService>().WithMetadata("Key", 3);
                c.Export<MultipleService4>().As<IMultipleService>().WithMetadata("Key", 4);
                c.Export<MultipleService5>().As<IMultipleService>().WithMetadata("Key", 5);
            });

            var dependentService = container.Locate<DependentService<IEnumerable<Meta<IMultipleService>>>>();

            var multipleServices = dependentService.Value.ToArray();

            Assert.Equal(5, multipleServices.Length);

            Assert.Equal(1, multipleServices[0].Metadata["Key"]);
            Assert.IsType<MultipleService1>(multipleServices[0].Value);

            Assert.Equal(2, multipleServices[1].Metadata["Key"]);
            Assert.IsType<MultipleService2>(multipleServices[1].Value);

            Assert.Equal(3, multipleServices[2].Metadata["Key"]);
            Assert.IsType<MultipleService3>(multipleServices[2].Value);

            Assert.Equal(4, multipleServices[3].Metadata["Key"]);
            Assert.IsType<MultipleService4>(multipleServices[3].Value);

            Assert.Equal(5, multipleServices[4].Metadata["Key"]);
            Assert.IsType<MultipleService5>(multipleServices[4].Value);
        }
    }
}
