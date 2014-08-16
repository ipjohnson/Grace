using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    public interface ICustomConstructorEnrichmentLinqExpressionProvider
    {
        IEnumerable<Expression> ProvideConstructorExpressions(ICustomEnrichmentLinqExpressionContext context);
    }
}
