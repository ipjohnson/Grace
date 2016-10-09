using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public class SimpleKnownValueExpression : IKnownValueExpression
    {
        private readonly Expression _expression;
        public SimpleKnownValueExpression(Type activationType, Expression expression)
        {
            ActivationType = activationType;
            _expression = expression;
        }

        public Type ActivationType { get; }

        public IActivationExpressionResult ValueExpression(IActivationExpressionRequest request)
        {
            return request.Services.Compiler.CreateNewResult(request, _expression);
        }
    }
}
