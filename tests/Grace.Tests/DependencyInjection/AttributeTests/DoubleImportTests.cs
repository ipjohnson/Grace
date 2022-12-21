using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Xunit;

namespace Grace.Tests.DependencyInjection.AttributeTests
{
    public class DoubleImportTests
    {
        [Fact]
        public void DoubleImportTest()
        {
            var container = new DependencyInjectionContainer();

            var assembly = Assembly.GetAssembly(typeof(Test));

            container.Configure(c => c
                .ExportAssembly(assembly).ExportAttributedTypes().Where(TypesThat.AreInTheSameNamespaceAs(typeof(Test)))
            );

            var test = container.Locate<ITest>();

            Assert.Equal(1, test.C);
            Assert.Equal(1, test.P);
            Assert.Equal(1, test.I);
        }

        #region class setup
        public class Exported
        { }

        public interface ITest
        {
            int P { get; }
            int I { get; }
            int C { get; }
        }

        [Export(typeof(ITest))]
        public class Test : ITest
        {
            public int P { get; set; } = 0;
            private Exported _exported;

            [Import]
            public Exported InjectProperty
            {
                get => _exported;

                set
                {
                    _exported = value;
                    P++;
                }
            }

            public int I { get; set; } = 0;

            [Import]
            public void InjectMethod(Exported exported)
            {
                I++;
            }

            public int C { get; set; } = 0;
            public Test()
            {
                C++;
            }
        }
        #endregion
    }
}
