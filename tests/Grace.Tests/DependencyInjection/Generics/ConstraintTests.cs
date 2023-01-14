using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Generics;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Generics
{
    public class ConstraintTests
    {
        [Fact]
        public void Generic_Resolve_All_With_Constraint_Failing()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export(typeof(GenericConstraintServiceA<>)).As(typeof(IGenericConstraintService<>));
                c.Export(typeof(GenericConstraintBasicService<>)).As(typeof(IGenericConstraintService<>));
            });

            var instances = container.LocateAll<IGenericConstraintService<IMultipleService>>();

            Assert.NotNull(instances);
            Assert.Single(instances);
        }

        [Fact]
        public void Generic_Dependent_Constraints()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<BasicService>().As<IBasicService>();
                c.Export(typeof(GenericConstraintServiceA<>)).As(typeof(IGenericConstraintService<>));
                c.Export(typeof(GenericConstraintBasicService<>)).As(typeof(IGenericConstraintService<>));
            });

            var instance = container.Locate<DependentService<IGenericConstraintService<IMultipleService>>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.IsType<GenericConstraintServiceA<IMultipleService>>(instance.Value);
        }


        [Fact]
        public void Generic_Dependent_IEnumerable_Constraints()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export(typeof(GenericConstraintServiceA<>)).As(typeof(IGenericConstraintService<>));
                c.Export(typeof(GenericConstraintBasicService<>)).As(typeof(IGenericConstraintService<>));
            });

            var instance = container.Locate<DependentService<IEnumerable<IGenericConstraintService<IMultipleService>>>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.Single(instance.Value);
        }

        [Fact]
        public void MultipleOutOfOrderOpenGeneric()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(MultipleOutOfOrderOpenGeneric<,,,>)).ByInterfaces();
                c.Export<BasicService>().ByInterfaces();
            });

            var multipleOut =
                container.Locate<IMultipleOutOfOrderOpenGeneric<IBasicService, bool, string, DateTime>>();

            Assert.NotNull(multipleOut);
        }

        [Fact]
        public void MissingConstraintOnOutOfOrderOpen()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(MultipleOutOfOrderOpenGeneric<,,,>)).ByInterfaces();
                c.Export<BasicService>().ByInterfaces();
            });

            container.TryLocate(out IMultipleOutOfOrderOpenGeneric<DateTime, bool, string, IBasicService> multipleOut);

            Assert.Null(multipleOut);

        }

        [Fact]
        public void LocateConcreteOpenGeneric()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(BaseGenericClass<,,,>)));

            var resolvedInstance = container.Locate<BaseGenericClass<int, string, double, DateTime>>();

            Assert.NotNull(resolvedInstance);
        }

        [Fact]
        public void ResolveConcretePartiallyClosedGeneric()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(PartialClosedClass<,,>)).As(typeof(BaseGenericClass<,,,>)));

            var resolvedInstance = container.Locate<BaseGenericClass<int, DateTime, string, double>>();

            Assert.NotNull(resolvedInstance);
        }

        [Fact]
        public void ResolveEvenMoreClosedGeneric()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(EvenMoreClosedClass<,>)).As(typeof(BaseGenericClass<,,,>)));

            var resolvedInstance = container.Locate<BaseGenericClass<int, DateTime, string, double>>();

            Assert.NotNull(resolvedInstance);
        }

        [Fact]
        public void MetaDataOnOpenGeneric()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(BaseGenericClass<,,,>)).WithMetadata("Hello", "World"));

            var resolvedInstance = container.Locate<Meta<BaseGenericClass<int, string, double, DateTime>>>();

            Assert.NotNull(resolvedInstance);
            Assert.NotNull(resolvedInstance.Value);

            Assert.Equal("World", resolvedInstance.Metadata["Hello"]);
        }

        [Fact]
        public void PrioritizeSemiClosedGenerics()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(BaseGenericClass<,,,>));
                c.Export(typeof(PartialClosedClass<,,>)).As(typeof(BaseGenericClass<,,,>));
                c.Export(typeof(EvenMoreClosedClass<,>)).As(typeof(BaseGenericClass<,,,>));
                c.PrioritizePartiallyClosedGenerics();
            });

            var openObject = container.Locate<BaseGenericClass<int, string, double, DateTime>>();

            Assert.NotNull(openObject);

            var semiClosed = container.Locate<BaseGenericClass<int, string, double, double>>();

            Assert.NotNull(semiClosed);
            Assert.IsType<PartialClosedClass<int, string, double>>(semiClosed);

            var reallyClosed = container.Locate<BaseGenericClass<int, string, string, double>>();

            Assert.NotNull(reallyClosed);
            Assert.IsType<EvenMoreClosedClass<int, string>>(reallyClosed);
        }
        
        [Fact]
        public void BulkRegisterOpenGenericByInterface()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(GetType().GetTypeInfo().Assembly.ExportedTypes).
                ByInterface(typeof(IOpenGenericPartiallyClosedInterface<,,,>)).
                PrioritizePartiallyClosedGenerics());

            var openImpl = container.Locate<IOpenGenericPartiallyClosedInterface<string, int, DateTime, float>>();

            Assert.NotNull(openImpl);

            var partiallyClosed = container.Locate<IOpenGenericPartiallyClosedInterface<string, int, DateTime, string>>();

            Assert.NotNull(partiallyClosed);
            Assert.IsType<PartiallyClosedInterface<string, int, DateTime>>(partiallyClosed);

            var moreClosed = container.Locate<IOpenGenericPartiallyClosedInterface<string, int, double, string>>();

            Assert.NotNull(moreClosed);
            Assert.IsType<EvenMoreClosedInterface<string, int>>(moreClosed);
        }

        [Fact]
        public void BulkRegisterOpenGenericByInterfaces()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(GetType().GetTypeInfo().Assembly.ExportedTypes).
                ByInterfaces().
                PrioritizePartiallyClosedGenerics());

            var openImpl = container.Locate<IOpenGenericPartiallyClosedInterface<string, int, DateTime, float>>();

            Assert.NotNull(openImpl);

            var partiallyClosed = container.Locate<IOpenGenericPartiallyClosedInterface<string, int, DateTime, string>>();

            Assert.NotNull(partiallyClosed);
            Assert.IsType<PartiallyClosedInterface<string, int, DateTime>>(partiallyClosed);

            var moreClosed = container.Locate<IOpenGenericPartiallyClosedInterface<string, int, double, string>>();

            Assert.NotNull(moreClosed);
            Assert.IsType<EvenMoreClosedInterface<string, int>>(moreClosed);
        }

        [Fact]
        public void OpenGenericClassConstraintTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(ClassConstraint<,>)).As(typeof(IClassConstraint<,>));
                c.Export<BasicService>();
            });


            var instance = container.Locate<DependentService<IClassConstraint<Hub<BasicService>, BasicService>>>();

            Assert.NotNull(instance);
        }


        [Fact]
        public void InheritOpenGenericClassConstraintTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(ClassConstraint<,>)).As(typeof(IClassConstraint<,>));
                c.Export<InheritHub>();
                c.Export<BasicService>();
            });


            var instance = container.Locate<DependentService<IClassConstraint<InheritHub, BasicService>>>();

            Assert.NotNull(instance);
        }
    }
}
