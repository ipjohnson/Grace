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
												Select(type =>
												       {
													       return type.Name.EndsWith("LinqClass");
												       }).
												EnrichWithExpression(new EnrichmentHelper()));

			EnrichWithLinqClass instance = container.Locate<EnrichWithLinqClass>();

			Assert.NotNull(instance);

			Assert.Equal(5, instance.IntProp);
		}
	}
}
