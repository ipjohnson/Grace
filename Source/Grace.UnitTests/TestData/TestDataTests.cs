using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.TestData;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.TestData
{
    public class TestDataTests
    {
        [Fact]
        public void BasicPersonGenerationTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer { new TestDataModule() };

            var testData = container.Locate<ITestDataProvider>();

            var person = testData.Create<PersonClass>();
        }
    }
}
