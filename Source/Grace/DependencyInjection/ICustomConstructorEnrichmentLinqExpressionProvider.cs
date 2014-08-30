using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Implementing this interface allows you to provide a set of Expressions that will be used to construct the type
    /// This is to override the default construction behavior
    /// </summary>
    public interface ICustomConstructorEnrichmentLinqExpressionProvider
    {
        /// <summary>
        /// Provide a set of Expressions that will be used to construct the export.
        /// </summary>
        /// <param name="context">context that contains certain parameters</param>
        /// <returns>expressions</returns>
        IEnumerable<Expression> ProvideConstructorExpressions(ICustomConstructorEnrichmentLinqExpressionContext context);
    }
}
