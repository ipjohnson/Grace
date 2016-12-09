using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Diagnostics;
using Grace.Tests.Classes.Simple;
using NSubstitute;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.Diagnostics
{
    [SubFixtureInitialize]
    public class ActivationStrategyMetadataDebuggerViewTests
    {
        [Theory]
        [AutoData]
        public void ActivationStrategyMetadataDebuggerView_ActivationType(ActivationStrategyMetadataDebuggerView debugger,
                                                                    IActivationStrategyMetadata metadata)
        {
            metadata.ActivationType.Returns(typeof(BasicService));

            Assert.Equal(typeof(BasicService), debugger.ActivationType);
        }
        [Theory]
        [AutoData]
        public void ActivationStrategyMetadataDebuggerView_ExportAs(ActivationStrategyMetadataDebuggerView debugger,
                                                                    IActivationStrategyMetadata metadata)
        {
            metadata.ExportAs.Returns(new[] { typeof(IBasicService) });

            Assert.Equal(1, metadata.ExportAs.Count());
        }

        [Theory]
        [AutoData]
        public void ActivationStrategyMetadataDebuggerView_ExportAsKeyed(ActivationStrategyMetadataDebuggerView debugger,
                                                                    IActivationStrategyMetadata metadata)
        {
            metadata.ExportAsKeyed.Returns(new[] { new KeyValuePair<Type, object>(typeof(IBasicService), "Blah") });

            Assert.Equal(1, metadata.ExportAsKeyed.Count());
        }

        [Theory]
        [AutoData]
        public void ActivationStrategyMetadataDebuggerView_Data(ActivationStrategyMetadataDebuggerView debugger,
                                                                    IActivationStrategyMetadata metadata)
        {
            var list = new[] { new KeyValuePair<object, object>(typeof(IBasicService), "Blah") };

            metadata.GetEnumerator().Returns(list.GetEnumerator());

            var metaList = debugger.Data.ToArray();

            Assert.Equal(1, metaList.Length);
            Assert.Equal(typeof(IBasicService), metaList[0].Key);
            Assert.Equal("Blah", metaList[0].Value);

        }
    }
}
