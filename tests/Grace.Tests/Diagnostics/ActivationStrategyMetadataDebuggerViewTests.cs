using System;
using System.Collections.Generic;
using System.Linq;
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

            Assert.Single(metadata.ExportAs);
        }

        [Theory]
        [AutoData]
        public void ActivationStrategyMetadataDebuggerView_ExportAsKeyed(ActivationStrategyMetadataDebuggerView debugger,
                                                                    IActivationStrategyMetadata metadata)
        {
            metadata.ExportAsKeyed.Returns(new[] { new KeyValuePair<Type, object>(typeof(IBasicService), "Blah") });

            Assert.Single(metadata.ExportAsKeyed);
        }

        [Theory]
        [AutoData]
        public void ActivationStrategyMetadataDebuggerView_Data(ActivationStrategyMetadataDebuggerView debugger,
                                                                    IActivationStrategyMetadata metadata)
        {
            var list = new List<KeyValuePair<object,object>> { new KeyValuePair<object, object>(typeof(IBasicService), "Blah") };

            var enumerator = list.GetEnumerator();
            metadata.GetEnumerator().Returns(enumerator);

            var metaList = debugger.Data.ToArray();

            Assert.Single(metaList);
            Assert.Equal(typeof(IBasicService), metaList[0].Key);
            Assert.Equal("Blah", metaList[0].Value);

        }
    }
}
