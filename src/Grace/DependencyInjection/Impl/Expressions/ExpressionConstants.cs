using System.Linq.Expressions;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// represents a set of constants that can be used while creating linq expression
    /// </summary>
    public class ExpressionConstants : IExpressionConstants
    {
        /// <summary>
        /// Default expression constants
        /// </summary>
        public static readonly IExpressionConstants Default = new ExpressionConstants();

        private ExpressionConstants()
        {
            ScopeParameter = Expression.Parameter(typeof(IExportLocatorScope), "scope");

            InjectionContextParameter = Expression.Parameter(typeof(IInjectionContext), "injectionContext");

            RootDisposalScope = Expression.Parameter(typeof(IDisposalScope), "disposalScope");

            KeyParameter = Expression.Parameter(typeof(object), "key");
        }

        /// <summary>
        /// Root disposal scope
        /// </summary>
        public ParameterExpression RootDisposalScope { get; }

        /// <summary>
        /// Scope parameter
        /// </summary>
        public ParameterExpression ScopeParameter { get; }

        /// <summary>
        /// IInjectionContext parameter
        /// </summary>
        public ParameterExpression InjectionContextParameter { get; }

        /// <summary>
        /// Locate key parameter
        /// </summary>
        public ParameterExpression KeyParameter { get; }
    }
}
