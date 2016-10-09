using System.Linq.Expressions;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public class ExpressionConstants : IExpressionConstants
    {
        public static readonly IExpressionConstants Default = new ExpressionConstants();

        private ExpressionConstants()
        {
            ScopeParameter = Expression.Parameter(typeof(IExportLocatorScope), "scope");

            InjectionContextParameter = Expression.Parameter(typeof(IInjectionContext), "injectionContext");

            RootDisposalScope = Expression.Parameter(typeof(IDisposalScope), "disposalScope");
        }

        public ParameterExpression RootDisposalScope { get; }

        public ParameterExpression ScopeParameter { get; }

        public ParameterExpression InjectionContextParameter { get; }
    }
}
