using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
using Grace.Diagnostics;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
    public class AdvancedContainerTests
    {
        #region Metdata tests

        [Fact]
        public void LocateAllWithMetadataTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithMetadata("Metadata", "Group2");
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithMetadata("Metadata", "Group2");
            });
            var list = container.LocateAllWithMetadata<ISimpleObject>("Metadata");

            Assert.NotNull(list);

            Assert.Equal(5, list.Count());
        }

        [Fact]
        public void LocateAllWithMetadataFiltered()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithMetadata("Metadata", "Group2");
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithMetadata("Metadata", "Group2");
            });

            var list = container.LocateAllWithMetadata<ISimpleObject>("Metadata", "Group1");

            Assert.NotNull(list);

            Assert.Equal(3, list.Count());

        }

        #endregion
        
        #region new context

        [Fact]
        public void InNewContextTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
                                      {
                                          c.Export<ContextSingleton>()
                                            .ByInterfaces()
                                            .UsingLifestyle(new SingletonPerInjectionContextLifestyle());
                                          c.Export<ContextClassA>().ByInterfaces();
                                          c.Export<ContextClassB>().ByInterfaces().InNewContext();
                                          c.Export<ContextClassC>().ByInterfaces();
                                      });

            IContextClassA classA = container.Locate<IContextClassA>();

            Assert.NotNull(classA);
            Assert.NotNull(classA.ContextClassB);
            Assert.NotNull(classA.ContextClassB.ContextClassC);

            Assert.NotNull(classA.ContextSingleton);
            Assert.NotNull(classA.ContextClassB.ContextSingleton);
            Assert.NotNull(classA.ContextClassB.ContextClassC.ContextSingleton);

            Assert.NotSame(classA.ContextSingleton, classA.ContextClassB.ContextSingleton);
            Assert.NotSame(classA.ContextSingleton, classA.ContextClassB.ContextClassC.ContextSingleton);
            Assert.Same(classA.ContextClassB.ContextSingleton, classA.ContextClassB.ContextClassC.ContextSingleton);

        }

        #endregion

        #region BeginLifetimeScope

        [Fact]
        public void BeginLifetimeScope()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>().AndSingletonPerScope());

            IDisposableService service = container.Locate<IDisposableService>();

            Assert.NotNull(service);

            bool called = false;

            using (var scope = container.BeginLifetimeScope())
            {
                var secondService = scope.Locate<IDisposableService>();

                Assert.NotNull(secondService);
                Assert.NotSame(service, secondService);
                Assert.Same(secondService, scope.Locate<IDisposableService>());

                secondService.Disposing += (sender, args) => called = true;
            }

            Assert.True(called);
        }
        #endregion

        #region WithNamedCtorValue
        [Fact]
        public void WithNamedCtorValue()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();
            DateTime currentTime = DateTime.Now;

            container.Configure(c => c.Export(typeof(DateTimeImport)).WithNamedCtorValue(() => currentTime));

            DateTimeImport import = container.Locate<DateTimeImport>();

            Assert.NotNull(import);
            Assert.Equal(currentTime, import.CurrentTime);
        }

        [Fact]
        public void WithNamedCtorValueGeneric()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();
            DateTime currentTime = DateTime.Now;

            container.Configure(c => c.Export<DateTimeImport>().WithNamedCtorValue(() => currentTime));

            DateTimeImport import = container.Locate<DateTimeImport>();

            Assert.NotNull(import);
            Assert.Equal(currentTime, import.CurrentTime);
        }


        [Fact]
        public void WithNamedCtorValueGenericNow()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<NowDateTimeImport>().WithNamedCtorValue(() => DateTime.Now));

            NowDateTimeImport import = container.Locate<NowDateTimeImport>();

            Assert.NotNull(import);
            Assert.Equal(import.CurrentTime.Date, DateTime.Now.Date);
        }

        [Fact]
        public void ExportNamedValue()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();
            DateTime currentTime = DateTime.Now;

            container.Configure(c => c.Export<DateTimeImport>());
            container.Configure(c => c.ExportNamedValue(() => currentTime));

            DateTimeImport import = container.Locate<DateTimeImport>();

            Assert.NotNull(import);
            Assert.Equal(currentTime, import.CurrentTime);
        }
        #endregion

        #region WithInspectorFor

        [Fact]
        public void WithInspectorFor()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .WithInspectorFor<ISimpleObject>(e => e.AddMetadata("SimpleObject", e.ActivationType)));

            var list =
                container.LocateAll<ISimpleObject>(consider: ExportsThat.HaveMetadata("SimpleObject"));

            Assert.Equal(5, list.Count);

            list =
                container.LocateAll<ISimpleObject>(consider:
                    ExportsThat.HaveMetadata("SimpleObject", typeof(SimpleObjectA)));

            Assert.Equal(1, list.Count);
        }



        #endregion

        #region BasedOn Interface Test

        [Fact]
        public void BasedOnInterfaceTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                              .BasedOn<ISimpleObject>()
                                              .ByInterfaces());

            IEnumerable<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(5, simpleObjects.Count());
        }

        [Fact]
        public void BasedOnInterfaceByType()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                              .BasedOn<ISimpleObject>()
                                              .ByType());

            SimpleObjectA simpleObjectA = container.Locate<SimpleObjectA>();

            Assert.NotNull(simpleObjectA);
        }

        #endregion

        #region TypesThat tests

        [Fact]
        public void TypesThatAreBasedOn()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .Select(TypesThat.AreBasedOn<ISimpleObject>()));

            List<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(5, simpleObjects.Count);

        }

        [Fact]
        public void TypesThatAreBasedOnChainedFilter()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .Select(TypesThat.AreBasedOn<ISimpleObject>().And.HaveAttribute<SimpleFilterAttribute>()));

            List<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(3, simpleObjects.Count);

        }

        [Fact]
        public void TypesThatAreBasedOnComplexChainedFilter()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .Select(TypesThat.AreBasedOn(TypesThat.StartWith("ISimple"))));

            List<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(5, simpleObjects.Count);

        }

        [Fact]
        public void TypesThatHaveAttributeChainedFilter()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .Select(TypesThat.AreBasedOn(TypesThat.HaveAttribute(TypesThat.StartWith("Simple")))));

            List<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(5, simpleObjects.Count);

        }

        [Fact]
        public void FromThisAssemblyFilter()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly(consider: TypesThat.AreBasedOn(TypesThat.HaveAttribute(TypesThat.StartWith("Simple")))))
                                      .ByInterfaces());

            List<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(5, simpleObjects.Count);

        }

        [Fact]
        public void TypesThatHaveAttributeFilter()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .Select(TypesThat.HaveAttribute(TypesThat.StartWith("Simple"))));

            List<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(3, simpleObjects.Count);

        }
        #endregion

        #region Prioritize

        [Fact]
        public void PrioritizeTypesThat()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .Select(TypesThat.AreBasedOn<ISimpleObject>())
                                      .Prioritize(TypesThat.EndWith("C")));

            var simpleObject = container.Locate<ISimpleObject>();

            Assert.NotNull(simpleObject);
            Assert.IsType<SimpleObjectC>(simpleObject);
        }

        #endregion

        #region Lifestyle Tests

        [Fact]
        public void LifestyleSingleton()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>()
                                      .As<IBasicService>()
                                      .Lifestyle.Singleton());

            IBasicService basicService = container.Locate<IBasicService>();

            Assert.NotNull(basicService);
            Assert.Same(basicService, container.Locate<IBasicService>());
        }

        [Fact]
        public void LifestyleSingletonPerScope()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>()
                                      .As<IBasicService>()
                                      .Lifestyle.SingletonPerScope());

            IBasicService basicService = container.Locate<IBasicService>();

            Assert.NotNull(basicService);
            Assert.Same(basicService,container.Locate<IBasicService>());

            using (var scope = container.BeginLifetimeScope())
            {
                IBasicService basicService2 = scope.Locate<IBasicService>();

                Assert.NotNull(basicService2);
                Assert.Same(basicService2,scope.Locate<IBasicService>());
                Assert.NotSame(basicService,basicService2);
            }
        }

        [Fact]
        public void BulkLifestyleSingleton()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterface<IBasicService>()
                                      .Lifestyle.Singleton());

            IBasicService basicService = container.Locate<IBasicService>();

            Assert.NotNull(basicService);
            Assert.Same(basicService, container.Locate<IBasicService>());
        }

        [Fact]
        public void BulkLifestyleSingletonPerScope()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterface<IBasicService>()
                                      .Lifestyle.SingletonPerScope());

            IBasicService basicService = container.Locate<IBasicService>();

            Assert.NotNull(basicService);
            Assert.Same(basicService, container.Locate<IBasicService>());

            using (var scope = container.BeginLifetimeScope())
            {
                IBasicService basicService2 = scope.Locate<IBasicService>();

                Assert.NotNull(basicService2);
                Assert.Same(basicService2, scope.Locate<IBasicService>());
                Assert.NotSame(basicService, basicService2);
            }
        }

        #endregion

        #region PropertyInspector

        [Fact]
        public void PropertyInjectorInspector()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.AddStrategyInspector(new PropertyInjectionInspector<IBasicService>());

            container.Configure(c =>
                                {
                                    c.Export<BasicService>().As<IBasicService>();
                                    c.Export<ImportPropertyService>().As<IImportPropertyService>();
                                });

            IImportPropertyService propertyService = container.Locate<IImportPropertyService>();

            Assert.NotNull(propertyService);
            Assert.NotNull(propertyService.BasicService);
        }

        [Fact]
        public void PropertyInjectorInspectorChildScope()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.AddStrategyInspector(new PropertyInjectionInspector<IBasicService>());

            var child = container.CreateChildScope();

            child.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<ImportPropertyService>().As<IImportPropertyService>();
            });

            IImportPropertyService propertyService = child.Locate<IImportPropertyService>();

            Assert.NotNull(propertyService);
            Assert.NotNull(propertyService.BasicService);
        }

        #endregion
    }
}
