using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    /// <summary>
    /// Wrapper strategy for strongly typed metadata
    /// </summary>
    public class StronglyTypedMetadataWrapperStrategy : BaseWrapperStrategy
    {
        private readonly IStrongMetadataInstanceProvider _strongMetadataInstanceProvider;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="strongMetadataInstanceProvider"></param>
        public StronglyTypedMetadataWrapperStrategy(IInjectionScope injectionScope,
            IStrongMetadataInstanceProvider strongMetadataInstanceProvider) : base (typeof(Meta<,>), injectionScope)
        {
            _strongMetadataInstanceProvider = strongMetadataInstanceProvider;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType">type being activated</param>
        /// <param name="injectionScope">scope for strategy</param>
        public StronglyTypedMetadataWrapperStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
        {
        }

        /// <summary>
        /// Get the type that is being wrapped
        /// </summary>
        /// <param name="type">requested type</param>
        /// <returns>wrapped type</returns>
        public override Type GetWrappedType(Type type)
        {
            if (type.IsConstructedGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(Meta<,>))
                {
                    return type.GetTypeInfo().GenericTypeArguments[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var requestType = request.ActivationType.GetTypeInfo().GenericTypeArguments[0];

            var constructor = request.ActivationType.GetTypeInfo().DeclaredConstructors.First();

            var newRequest = request.NewRequest(requestType, this, request.ActivationType, RequestType.Other, null, true, true);

            var strategy = request.GetWrappedExportStrategy();

            if (strategy == null)
            {
                throw new Exception("Could not find export stragegy to get metadata from");
            }

            var expressionResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

            var metadata =
                _strongMetadataInstanceProvider.GetMetadata(request.ActivationType.GenericTypeArguments[1], strategy.Metadata);

            var newExpression =
                Expression.New(constructor, expressionResult.Expression, Expression.Constant(metadata));

            var newResult = request.Services.Compiler.CreateNewResult(request, newExpression);

            newResult.AddExpressionResult(expressionResult);

            return newResult;
        }
        
        /// <summary>
        /// Compiles delegate
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="compiler"></param>
        /// <param name="activationType"></param>
        /// <returns></returns>
        protected override ActivationStrategyDelegate CompileDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler,
            Type activationType)
        {
            var request = compiler.CreateNewRequest(activationType, 1, scope);

            var expressionResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, request);

            return compiler.CompileDelegate(scope, expressionResult);
        }
    }
}
