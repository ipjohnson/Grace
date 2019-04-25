using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ScopeExtensions
{
    // ReSharper disable once InconsistentNaming
    public class IExportLocatorScopeExtensionsTests
    {
        [Fact]
        public void IExportLocatorScopeExtensions_GetInjectionScope()
        {
            var container = new DependencyInjectionContainer();

            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                Assert.NotNull(lifetimeScope);

                Assert.Same(container, lifetimeScope.GetInjectionScope());
            }
        }
        
        public class ImportPropertyClass
        {
            public IBasicService BasicService { get; set; }
        }

        [Fact]
        public void IExportLocatorScopeExtensions_WhatDoIHave()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>().Lifestyle.Singleton();
                c.Export<DependentService<IBasicService>>().As<IDependentService<IBasicService>>();
                c.Export<ImportPropertyClass>().ImportProperty(i => i.BasicService);
                c.Export<MultipleService1>().AsKeyed<IMultipleService>(1);
            });

            using (var child = container.CreateChildScope())
            {
                var whatDoIHave = child.WhatDoIHave(consider: s => true);

                Assert.False(string.IsNullOrEmpty(whatDoIHave));

                Assert.Contains("Singleton", whatDoIHave);
                Assert.Contains("Member Name", whatDoIHave);
                Assert.Contains( "As Keyed Type: 1", whatDoIHave);
            }
        }
    }
}
