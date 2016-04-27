using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
	public class EnrichmentWithTests
	{
		public class EnrichmentHelper : ICustomEnrichmentLinqExpressionProvider
		{
			public IEnumerable<Expression> ProvideExpressions(ICustomEnrichmentLinqExpressionContext context)
			{
				PropertyInfo propertyInfo = context.ActivationType.GetProperty("IntProp");

				if(propertyInfo != null)
				{
					yield return Expression.Assign(Expression.Property(context.InstanceVariable, propertyInfo), Expression.Constant(5));
				}
			}
		}

		[Fact]
		public void EnrichWithExpressionTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<EnrichWithLinqClass>().EnrichWithExpression(new EnrichmentHelper()));

			EnrichWithLinqClass instance = container.Locate<EnrichWithLinqClass>();

			Assert.NotNull(instance);

			Assert.Equal(5, instance.IntProp);
		}

		[Fact]
		public void EnrichSetWithExpression()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly()).
												ByType().
												Select(type => type.Name.EndsWith("LinqClass")).
												EnrichWithExpression(new EnrichmentHelper()));

			EnrichWithLinqClass instance = container.Locate<EnrichWithLinqClass>();

			Assert.NotNull(instance);

			Assert.Equal(5, instance.IntProp);
		}

		[Fact]
		public void EnrichmentDelegateForSetTest()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export(Types.FromThisAssembly()).
												ByType().
												Select(type => type.Name.EndsWith("LinqClass")).
												EnrichWith((scope, context, export) =>
												           {
													           IIntPropClass propClass = export as IIntPropClass;

													           if (propClass != null)
													           {
														           propClass.IntProp = 5;
													           }

													           return export;
												           }));

			EnrichWithLinqClass instance = container.Locate<EnrichWithLinqClass>();

			Assert.NotNull(instance);

			Assert.Equal(5, instance.IntProp);
		}
        
        [Fact]
        public void EnrichmentDelegateForTypedSetTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly()).
                                                ByType().
                                                Select(type => type.Name.EndsWith("LinqClass")).
                                                EnrichWithTyped<IIntPropClass>(x => { x.IntProp = 5; return x; }));

            EnrichWithLinqClass instance = container.Locate<EnrichWithLinqClass>();

            Assert.NotNull(instance);

            Assert.Equal(5, instance.IntProp);
        }

        [Fact]
        public void EnrichmentDelegateForTypedSetWithScopeAndContextTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly()).
                                                ByType().
                                                Select(type => type.Name.EndsWith("LinqClass")).
                                                EnrichWithTyped<IIntPropClass>((scope, context, x) => { x.IntProp = 5; return x; }));

            EnrichWithLinqClass instance = container.Locate<EnrichWithLinqClass>();

            Assert.NotNull(instance);

            Assert.Equal(5, instance.IntProp);
        }

        [Fact]
        public void EnrichWithTypeTest()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c => c.ExportAs<ImportPropertyService, IImportPropertyService>().
                                       EnrichWithTyped(s => { s.BasicService = basicService; return s; }));

            var propertyService = container.Locate<IImportPropertyService>();

            Assert.NotNull(propertyService);
            Assert.Same(basicService, propertyService.BasicService);
        }


        [Fact]
        public void ApplyTest()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c => c.ExportAs<ImportPropertyService, IImportPropertyService>().
                                       Apply(s =>  s.BasicService = basicService));

            var propertyService = container.Locate<IImportPropertyService>();

            Assert.NotNull(propertyService);
            Assert.Same(basicService, propertyService.BasicService);
        }

        [Fact]
        public void ApplySetTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly()).
                                                ByType().
                                                Select(type => type.Name.EndsWith("LinqClass")).
                                                Apply<IIntPropClass>(x => x.IntProp = 5));

            EnrichWithLinqClass instance = container.Locate<EnrichWithLinqClass>();

            Assert.NotNull(instance);

            Assert.Equal(5, instance.IntProp);
        }


        [Fact]
        public void ApplySetWithScopeAndContextTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly()).
                                                ByType().
                                                Select(type => type.Name.EndsWith("LinqClass")).
                                                Apply<IIntPropClass>((scope,context,x) => x.IntProp = 5));

            EnrichWithLinqClass instance = container.Locate<EnrichWithLinqClass>();

            Assert.NotNull(instance);

            Assert.Equal(5, instance.IntProp);
        }
    }
}
