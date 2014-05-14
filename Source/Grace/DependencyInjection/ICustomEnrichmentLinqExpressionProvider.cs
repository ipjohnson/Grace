using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Classes the implement this interface can be used to enrich the Linq expression that the DI container creates for an export
	/// </summary>
	public interface ICustomEnrichmentLinqExpressionProvider
	{
		/// <summary>
		/// Provide a list of linq expressions that will be added to the Linq expression tree
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		[NotNull]
		IEnumerable<Expression> ProvideExpressions(ICustomEnrichmentLinqExpressionContext context);
	}
}
