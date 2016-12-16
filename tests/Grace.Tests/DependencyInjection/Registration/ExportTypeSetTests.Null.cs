using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    [SubFixtureInitialize]
    public partial class ExportTypeSetTests
    {
        [Theory]
        [AutoData]
        public void ExportTypeSet_AndCondition_Null_Throws(ExportTypeSetConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.AndCondition(null));
        }

        [Theory]
        [AutoData]
        public void ExportTypeSet_BasedOn_Null_Throws(ExportTypeSetConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.BasedOn(null));
        }
        
        [Theory]
        [AutoData]
        public void ExportTypeSet_ByInterface_Null_Throws(ExportTypeSetConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.ByInterface(null));
        }
    
        [Theory]
        [AutoData]
        public void ExportTypeSet_ByTypes_Null_Throws(ExportTypeSetConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.ByTypes(null));
        }

        [Theory]
        [AutoData]
        public void ExportTypeSet_ByKeyed_Null_Throws(ExportTypeSetConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.ByKeyedTypes(null));
        }
        
        [Theory]
        [AutoData]
        public void ExportTypeSet_Exclude_Null_Throws(ExportTypeSetConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.Exclude(null));
        }

        [Theory]
        [AutoData]
        public void ExportTypeSet_UsingLifestyle_Null_Throws(ExportTypeSetConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.UsingLifestyle((Func<Type,ICompiledLifestyle>)null));
        }

        [Theory]
        [AutoData]
        public void ExportTypeSet_Where_Null_Throws(ExportTypeSetConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.Where(null));
        }

        [Theory]
        [AutoData]
        public void ExportTypeSet_WithInspector_Null_Throws(ExportTypeSetConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.WithInspector(null));
        }
    }
}
